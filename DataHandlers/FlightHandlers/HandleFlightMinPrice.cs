using System.Text.Json;
using TravelBridgeAPI.Models.FlightModels.FlightMinPrice;

namespace TravelBridgeAPI.DataHandlers.FlightHandlers
{
    public class HandleFlightMinPrice
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly ILogger<HandleFlightMinPrice> _logger;

        private int _logCount = 200;

        public HandleFlightMinPrice(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ApiKeyManager apiKey,
            ILogger<HandleFlightMinPrice> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Rootobject?> GetMinFlightPriceAsync(
            string from, 
            string to, 
            string departure, 
            string? returnFlight, 
            string? cabinClass, 
            string? currencyCode)
        {
            _logCount++;
            if(_logCount == 300)
            {
                _logCount = 200; // Resetting logcount after 200 logs
            }

            _logger.LogInformation("Fetching minimum flight price started {@MinPriceRequestInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                From = from,
                To = to,
                Departure = departure,
                ReturnFlight = returnFlight,
                CabinClass = cabinClass,
                CurrencyCode = currencyCode
            });

            var rootObject = await GetMinFlightPriceFromAPI(from, to, departure, returnFlight, cabinClass, currencyCode);
            if (rootObject != null)
            {
                _logger.LogInformation("Successfully fetched minimum flight price {@MinPriceSuccessInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    From = from,
                    To = to,
                    Departure = departure
                });

                return rootObject;
            }
            _logger.LogWarning("No minimum flight price found {@MinPriceWarningInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                From = from,
                To = to,
                Departure = departure
            });
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

            _logger.LogInformation("Calling external flight price API {@ExternalApiCallInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                Url = url
            });

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
                _logger.LogError(ex, "Error calling external flight price API {@ExternalApiErrorInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    Url = url
                });

                throw new Exception($"Error fetching flight details: {ex.Message}");
            }
        }
    }
}
