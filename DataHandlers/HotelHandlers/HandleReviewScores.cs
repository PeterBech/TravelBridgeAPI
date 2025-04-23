using System.Text.Json;
using TravelBridgeAPI.Models.HotelModels.HotelReviewScores;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleReviewScores
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly ILogger<HandleReviewScores> _logger;
        private int _logCount = 700;

        public HandleReviewScores(
            HttpClient httpClient,
            IConfiguration configuration,
            ApiKeyManager apiKey,
            ILogger<HandleReviewScores> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Rootobject?> GetHotelReviewScores(int id, string? language)
        {
            _logCount++;
            if (_logCount == 801)
                _logCount = 700;

            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}\n" +
                $" Request started: {DateTime.Now}\n" +
                $" Fetching Hotel Review Scores for:\n" +
                $" - Hotel ID: {id}\n" +
                $" - Language: {language ?? "default"}");

            var hotelReviewScores = await SearchHotelReviewScoresAsync(id, language);

            if (hotelReviewScores != null)
            {
                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}\n" +
                    $" Request ended: {DateTime.Now}\n" +
                    $" Successfully fetched review scores for Hotel ID: {id}");
                return hotelReviewScores;
            }

            _logger.LogWarning(
                $"[LOG] Log num: {_logCount}\n" +
                $" Request ended: {DateTime.Now}\n" +
                $" No review scores found for Hotel ID: {id}");
            return null;
        }

        private async Task<Rootobject?> SearchHotelReviewScoresAsync(int id, string? language)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/hotels/getHotelReviewScores?hotel_id={id}";

            if (!string.IsNullOrEmpty(language))
                url += $"&languagecode={language}";

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
                        $" Failed to fetch review scores - Status: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var reviewScores = JsonSerializer.Deserialize<Rootobject>(jsonResponse);

                if (reviewScores == null)
                {
                    _logger.LogWarning(
                        $"[LOG] Log num: {_logCount}\n" +
                        $" Deserialization returned null for Hotel ID: {id}");
                    return null;
                }

                return reviewScores;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    $"[LOG] Log num: {_logCount}\n" +
                    $" Timestamp: {DateTime.Now}\n" +
                    $" Error fetching review scores: {ex.Message}");
                throw;
            }
        }
    }
}
