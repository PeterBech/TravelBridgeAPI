using System.Text.Json;
using TravelBridgeAPI.Models.FlightModels.FlightDetails;

namespace TravelBridgeAPI.DataHandlers.FlightHandlers
{
    public class HandleFlightDetails
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly ILogger<HandleFlightDetails> _logger;
        private int _logCount = 100;

        public HandleFlightDetails(
            HttpClient httpClient,
            IConfiguration configuration,
            ApiKeyManager apiKey,
            ILogger<HandleFlightDetails> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Rootobject?> GetFlightDetailsAsync(string token, string currencyCode)
        {
            _logCount++;
            if (_logCount == 201)
                _logCount = 100; // Reset the counter after 100 requests

            _logger.LogInformation("Fetching flight details started {@FlightDetailsRequestInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                Token = token,
                CurrencyCode = currencyCode
            });

            var flightDetails = await GetFlightDetailsFromAPI(token, currencyCode);

            if (flightDetails != null)
            {
                _logger.LogInformation("Fetching flight details succeeded {@FlightDetailsSuccessInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    Token = token,
                    CurrencyCode = currencyCode
                });

                return flightDetails;
            }

            _logger.LogWarning("No flight details found {@FlightDetailsWarningInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                Token = token,
                CurrencyCode = currencyCode
            });

            return null;
        }

        private async Task<Rootobject?> GetFlightDetailsFromAPI(string token, string currencyCode)
        {
            _logger.LogInformation("Calling external flight details API {@ExternalApiCallInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                Token = token
            });

            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];

            string curency = string.IsNullOrWhiteSpace(currencyCode) ? "EUR" : currencyCode;
            string url = $"https://{apiHost}/api/v1/flights/getFlightDetails?token={token}&currency_code={curency}";

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
                _logger.LogError(ex, "Error calling external flight details API {@ExternalApiErrorInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    Url = url,
                    Token = token
                });

                throw new Exception($"Error fetching flight details: {ex.Message}");
            }
        }
    }
}
