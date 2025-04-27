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

            _logger.LogInformation("Fetching hotel review scores started {@HotelReviewRequestInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                HotelId = id,
                Language = language
            });

            var hotelReviewScores = await SearchHotelReviewScoresAsync(id, language);

            if (hotelReviewScores != null)
            {
                _logger.LogInformation("Successfully fetched hotel review scores {@HotelReviewSuccessInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    HotelId = id
                });
                return hotelReviewScores;
            }

            _logger.LogWarning("No hotel review scores found {@HotelReviewWarningInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                HotelId = id
            });
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
                    _logger.LogWarning("Failed to fetch hotel review scores {@HotelReviewFetchWarningInfo}", new
                    {
                        LogNumber = _logCount,
                        Timestamp = DateTime.UtcNow,
                        response.StatusCode
                    });
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var reviewScores = JsonSerializer.Deserialize<Rootobject>(jsonResponse);

                if (reviewScores == null)
                {
                    _logger.LogWarning("Deserialization returned null {@HotelReviewDeserializationWarning}", new
                    {
                        LogNumber = _logCount,
                        Timestamp = DateTime.UtcNow,
                        HotelId = id
                    });
                    return null;
                }

                return reviewScores;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching hotel review scores {@HotelReviewErrorInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    HotelId = id
                });
                throw;
            }
        }
    }
}
