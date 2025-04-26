using System.Text.Json;
using TravelBridgeAPI.Models.HotelModels.HotelPhotos;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleHotelPhotos
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly ILogger<HandleHotelPhotos> _logger;
        private int _logCount = 600;

        public HandleHotelPhotos(
            HttpClient httpClient,
            IConfiguration configuration,
            ApiKeyManager apiKeyManager,
            ILogger<HandleHotelPhotos> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKeyManager ?? throw new ArgumentNullException(nameof(apiKeyManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Rootobject?> GetHotelPhotos(int hotelId)
        {
            _logCount++;
            if (_logCount == 701)
                _logCount = 600; 

            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}\n" +
                $" Request started: {DateTime.Now}\n" +
                $" Fetching hotel photos for hotel ID: {hotelId}");

            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/hotels/getHotelPhotos?hotel_id={hotelId}";

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

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        $"[LOG] Log num: {_logCount}\n" +
                        $" Request ended: {DateTime.Now}\n" +
                        $" Failed to fetch hotel photos - Status: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var hotelPhotos = JsonSerializer.Deserialize<Rootobject>(jsonResponse);

                if (hotelPhotos == null)
                {
                    _logger.LogWarning(
                        $"[LOG] Log num: {_logCount}\n" +
                        $" Deserialization returned null for hotel ID: {hotelId}");
                    return null;
                }

                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}\n" +
                    $" Request ended: {DateTime.Now}\n" +
                    $" Successfully fetched hotel photos for hotel ID: {hotelId}");

                return hotelPhotos;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    $"[LOG] Log num: {_logCount}\n" +
                    $" Timestamp: {DateTime.Now}\n" +
                    $" Error fetching hotel photos: {ex.Message}");
                throw;
            }
        }
    }
}
