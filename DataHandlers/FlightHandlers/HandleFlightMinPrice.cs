using System.Text.Json;
using TravelBridgeAPI.Models.FlightModels.FlightMinPrice;

namespace TravelBridgeAPI.DataHandlers.FlightHandlers
{
    public class HandleFlightMinPrice
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;

        public HandleFlightMinPrice(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<Rootobject?> GetMinFlightPriceAsync(
            string from, 
            string to, 
            string departure, 
            string? returnFlight, 
            string? cabinClass, 
            string? currencyCode)
        {

            var rootObject = await GetMinFlightPriceFromAPI(from, to, departure, returnFlight, cabinClass, currencyCode);
            if (rootObject != null)
            {
                return rootObject;
            }

            return null;
        }

        private async Task<Rootobject?> GetMinFlightPriceFromAPI(
            string from,
            string to,
            string departure,
            string? returnFlight,
            string? cabinClass,
            string? currencyCode)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];

            var queryParams = new List<string>
            {
                $"fromId={from}",
                $"toId={to}",
                $"departDate={departure}"
            };

            if (!string.IsNullOrWhiteSpace(returnFlight))
                queryParams.Add($"returnDate={returnFlight}");

            if (!string.IsNullOrWhiteSpace(cabinClass))
                queryParams.Add($"cabinClass={cabinClass}");

            if (!string.IsNullOrWhiteSpace(currencyCode))
                queryParams.Add($"currency_code={currencyCode}");

            string url = $"https://{apiHost}/api/v1/flights/getMinPrice?" + string.Join("&", queryParams);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "x-rapidapi-key", apiKey },
                    { "x-rapidapi-host", apiHost }
                },
            };

            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var flightMinPrice = JsonSerializer.Deserialize<Rootobject>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return flightMinPrice;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching flight details: {ex.Message}");
            }
        }
    }
}
