using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json;
using TravelBridgeAPI.Models.FlightModels.FlightSearches;
using TravelBridgeAPI.Security;

namespace TravelBridgeAPI.DataHandlers.FlightHandlers
{
    public class HandleSearch
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IApiKeyValidation _apiKeyValidation;
        private readonly ApiKeyManager _apiKeyManager;

        public HandleSearch(HttpClient httpClient, IConfiguration configuration, HandleLocations handleLocations, ApiKeyManager apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<Rootobject?> GetDirectFlightAsync(
            string fromId,
            string toId,
            string date,
            string? sort = null,
            string? cabinClass = null,
            string? currency = null)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];

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
