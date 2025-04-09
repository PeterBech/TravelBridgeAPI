using TravelBridgeAPI.Models.HotelReviewScores;
using System.Text.Json;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleReviewScores
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;

        public HandleReviewScores(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<Rootobject?> GetHotelReviewScores(int id, string? language)
        {
            var hotelReviewScores = await SearchHotelReviewScoresAsync(id, language);
            if (hotelReviewScores != null)
            {
                return hotelReviewScores;
            }
            return null;
        }

        public async Task<Rootobject?> SearchHotelReviewScoresAsync(int id, string? language)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/hotels/getHotelReviewScores?hotel_id={id}";
            if(language != null)
                url += $"&languagecode={language}";

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

            using(var response = await _httpClient.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Rootobject>(jsonResponse);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return null;
                }
            }
        }
    }
}
