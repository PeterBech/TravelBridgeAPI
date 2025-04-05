using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json;
using TravelBridgeAPI;
using TravelBridgeAPI.DataHandlers;
using TravelBridgeAPI.Models.FlightSearches;

namespace TravelBridgeAPI.DataHandlers
{
    public class HandleSearch
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly HandleLocations _handleLocations;

        private readonly ApiKeyManager _apiKeyManager;

        public HandleSearch(HttpClient httpClient, IConfiguration configuration, HandleLocations handleLocations, ApiKeyManager apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _handleLocations = handleLocations ?? throw new ArgumentNullException(nameof(handleLocations));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<Rootobject?> GetDirectFlightAsync(
            string departureIata,
            string arrivalIata,
            string date,
            string? sort = null,
            string? cabinClass = null,
            string? currency = null)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];

            var departureLocation = await _handleLocations.GetLocationAsync(departureIata);
            var arrivalLocation = await _handleLocations.GetLocationAsync(arrivalIata);

            if (departureLocation?.data == null || arrivalLocation?.data == null)
            {
                Console.WriteLine("Could not retrieve valid location data.");
                return null;
            }

            string? fromId = departureLocation.data.FirstOrDefault(d => d.type == "AIRPORT")?.id;
            string? toId = arrivalLocation.data.FirstOrDefault(d => d.type == "AIRPORT")?.id;

            if (string.IsNullOrEmpty(fromId) || string.IsNullOrEmpty(toId))
            {
                Console.WriteLine("No valid airport IDs found.");
                return null;
            }

            string url = $"https://{apiHost}/api/v1/flights/searchFlights" +
                         $"?fromId={fromId}" +
                         $"&toId={toId}" +
                         $"&departDate={date}" +
                         $"&pageNo=1" +
                         $"&adults=1";

            if (!string.IsNullOrEmpty(sort)) url += $"&sort={sort}";
            if (!string.IsNullOrEmpty(cabinClass)) url += $"&cabinClass={cabinClass}";
            if (!string.IsNullOrEmpty(currency)) url += $"&currency_code={currency}";

            Console.WriteLine($"[DEBUG] Requesting: {url}");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "x-rapidapi-key", apiKey },
                    { "x-rapidapi-host", apiHost }
                }
            };

            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Raw response: {jsonString}");

                var flightRoutes = JsonSerializer.Deserialize<Rootobject?>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return flightRoutes;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Direct flight request failed: {ex.Message}");
                return null;
            }
        }
    }
}
