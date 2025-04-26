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
                _logCount = 900; // Resetting logcount after 100 logs
            }

            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}" +
                $" Request started: {DateTime.Now}" +
                $" - Fetching hotel destination for location: {location}.");
            var hotelDestination = await SearchHotelDestinationAsync(location);
            if (hotelDestination != null)
            {
                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}" +
                    $" Request ended: {DateTime.Now}" +
                    $" - Successfully fetched hotel destination.");
                return hotelDestination;
            }
            _logger.LogWarning(
                $"[LOG] Log num: {_logCount}" +
                $" Request ended: {DateTime.Now}" +
                $" - No hotel destination found.");
            return null;
        }

        private async Task<Rootobject?> SearchHotelDestinationAsync(string query)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/hotels/searchDestination?query={query}";
            
            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}" +
                $" Timestamp: {DateTime.Now}" +
                $" - Fetching hotel destination for query: {query}" +
                $" from external API.");

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
            using (var response = await _httpClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        $"[LOG] Log num: {_logCount}" +
                        $" Timestamp: {DateTime.Now}" +
                        $" - Successfully fetched hotel destination.");
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Rootobject>(jsonResponse);
                }
                else
                {
                    _logger.LogError(
                        $"[LOG] Log num: {_logCount}" +
                        $" Timestamp: {DateTime.Now}" +
                        $" - Failed to fetch hotel destination - Status: {response.StatusCode}");
                    return null;
                }
            }
        }
    }
}
