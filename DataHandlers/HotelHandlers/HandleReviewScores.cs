using TravelBridgeAPI.Models.HotelReviewScores;
using System.Text.Json;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleReviewScores
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;

        /// <summary>
        /// Initializes a new instance of the HandleReviewScores class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any required dependency (HttpClient, IConfiguration, or ApiKeyManager) is null.
        /// </exception>
        public HandleReviewScores(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        /// <summary>
        /// Asynchronously retrieves hotel review scores for the specified hotel and optional language code.
        /// </summary>
        /// <param name="id">The identifier of the hotel for which to retrieve review scores.</param>
        /// <param name="language">An optional language code (e.g., "en") used to localize the review scores.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Rootobject"/> instance with the hotel review scores if available; otherwise, null.
        /// </returns>
        public async Task<Rootobject?> GetHotelReviewScores(int id, string? language)
        {
            var hotelReviewScores = await SearchHotelReviewScoresAsync(id, language);
            if (hotelReviewScores != null)
            {
                return hotelReviewScores;
            }
            return null;
        }

        /// <summary>
        /// Asynchronously retrieves hotel review scores for the specified hotel.
        /// </summary>
        /// <param name="id">The hotel identifier for which to fetch review scores.</param>
        /// <param name="language">An optional language code to localize the review scores.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a <see cref="Rootobject"/>
        /// with the hotel review scores if the API call is successful; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// This method constructs an API request URL using configuration settings and the next API key,
        /// then sends a GET request to fetch the review scores. Upon a successful response, the JSON
        /// content is deserialized into a <see cref="Rootobject"/>. If the request fails, an error is logged
        /// and <c>null</c> is returned.
        /// </remarks>
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
