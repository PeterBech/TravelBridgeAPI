using System.Runtime.InteropServices;
using TravelBridgeAPI.Models.HotelDestination;
using System.Text.Json;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleSearchDestination
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;

        public HandleSearchDestination(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<Rootobject?> GetHotelDestination(string location)
        {
            var hotelDestination = await SearchHotelDestinationAsync(location);
            if (hotelDestination != null)
            {
                return hotelDestination;
            }
            return null;
        }

        private async Task<Rootobject?> SearchHotelDestinationAsync(string query)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/hotels/searchDestination?query={query}";
            Console.WriteLine($"SearchLocationAPI Key: {apiKey}");
            Console.WriteLine($"SearchLocationAPI BaseURL: {apiHost}");
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
