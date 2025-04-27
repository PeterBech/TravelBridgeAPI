using System.Runtime.InteropServices;
using System.Text.Json;
using TravelBridgeAPI.Models.HotelModels.HotelDestination;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleSearchDestination
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly ILogger<HandleSearchDestination> _logger;

        private int _logCount = 900;

        public HandleSearchDestination(
            HttpClient httpClient,
            IConfiguration configuration, ApiKeyManager apiKey,
            ILogger<HandleSearchDestination> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Rootobject?> GetHotelDestination(string location)
        {
            _logCount++;
            if (_logCount == 1001)
            {
                _logCount = 900; // Resetting logcount after 900 logs
            }

            _logger.LogInformation("Fetching hotel destination started {@HotelDestinationRequestInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                Location = location
            });

            var hotelDestination = await SearchHotelDestinationAsync(location);

            if (hotelDestination != null)
            {
                _logger.LogInformation("Successfully fetched hotel destination {@HotelDestinationSuccessInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    Location = location
                });
                return hotelDestination;
            }
            _logger.LogWarning("No hotel destination found {@HotelDestinationWarningInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                Location = location
            });
            return null;
        }

        private async Task<Rootobject?> SearchHotelDestinationAsync(string query)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/hotels/searchDestination?query={query}";

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                    Headers =
                {
                    { "X-RapidAPI-Key", apiKey },
                    { "X-RapidAPI-Host", apiHost }
                }
                };

                using var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Rootobject>(jsonString);
                }
                else
                {
                    _logger.LogError("Failed to fetch hotel destination {@HotelDestinationApiErrorInfo}", new
                    {
                        LogNumber = _logCount,
                        Timestamp = DateTime.UtcNow,
                        response.StatusCode
                    });
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling hotel destination API {@HotelDestinationExceptionInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow
                });
                throw;
            }
        }
    }
}