using System.Text.Json;
using TravelBridgeAPI.Models.HotelPhotos;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleHotelPhotos
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;

        public HandleHotelPhotos(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKeyManager)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKeyManager;
        }
        public async Task<Rootobject> GetHotelPhotos(int hotelId)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/hotels/getHotelPhotos?hotel_id={hotelId}";

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
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Rootobject>(jsonResponse);
            }

            Console.WriteLine($"Error (HotelPhotos): {response.StatusCode}");
            return null;
        }
    }
}