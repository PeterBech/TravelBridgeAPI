using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json;
using TravelBridgeAPI.Models.FlightDetails;

namespace TravelBridgeAPI.DataHandlers
{
    public class HandleFlightDetails
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;

        public HandleFlightDetails(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<Rootobject?> GetFlightDetails(string token, string currencyCode)
        {
            var flightDetails = await GetFlightDetailsFromAPI(token, currencyCode);
            if (flightDetails != null)
            {
                return flightDetails;
            }

            return null;
        }

        private async Task<Rootobject?> GetFlightDetailsFromAPI(string token, string currencyCode)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/flights/getFlightDetails?token={token}&currency_code={currencyCode}";

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
                var flightDetails = JsonSerializer.Deserialize<Rootobject>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return flightDetails;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching flight details: {ex.Message}");
            }
        }
    }
}
