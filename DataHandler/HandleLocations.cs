using System.Text.Json;
using TravelBridgAPI.Models.FlightLocations;

namespace TravelBridgAPI.DataHandler
{
    public class HandleLocations
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HandleLocations(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<Rootobject?> GetLocationAsync(string city)
        {
            var rootObject = await GetLocationFromDb(city);
            if (rootObject != null)
            {
                return rootObject;
            }

            rootObject = await SearchLocationAsync(city);
            if (rootObject != null)
            {
                SaveLocationToDb(rootObject);
                return rootObject;
            }

            return null;
        }

        private async Task<Rootobject?> GetLocationFromDb(string city)
        {
            return null;
        }

        private async Task SaveLocationToDb(Rootobject location)
        {
        }

        private async Task<Rootobject?> SearchLocationAsync(string query)
        {
            string apiKey = _configuration["RapidApi:ApiKey"];
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/flights/searchDestination?query={query}";

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
                var flightLocations = JsonSerializer.Deserialize<Rootobject>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return flightLocations;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching flight destination: {ex.Message}");
            }
        }
    }
}
