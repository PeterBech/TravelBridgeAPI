using System.Text.Json;
using TravelBridgeAPI.Models.FlightMinPrice;

namespace TravelBridgeAPI.DataHandlers
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

        public async Task<Rootobject?> GetMinFlightPrice(string from, string to, string departure, string returnFlight, string cabinClass, string curencyCode)
        {
            var rootObject = await GetMinFlightPriceFromDb(from, to);
            if (rootObject != null)
            {
                return rootObject;
            }

            rootObject = await GetMinFlightPriceFromAPI(from, to, departure, returnFlight, cabinClass, curencyCode);
            if (rootObject != null)
            {
                SaveMinFlightPriceToDb(rootObject);
                return rootObject;
            }

            return null;
        }

        private async Task<Rootobject?> GetMinFlightPriceFromDb(string from, string to)
        {
            return null;
        }

        private async Task SaveMinFlightPriceToDb(Rootobject flightMinPrice)
        {
        }

        private async Task<Rootobject?> GetMinFlightPriceFromAPI(string from, string to, string departure, string returnFlight, string cabinClass, string curencyCode)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RAPIDAPI_BASE_URL"];
            string url = $"https://{apiHost}/api/v1/flights/getMinPrice?fromId={from}&toId={to}&departDate={departure}&returnDate={returnFlight}&cabinClass={cabinClass}&currency_code={curencyCode}";


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
