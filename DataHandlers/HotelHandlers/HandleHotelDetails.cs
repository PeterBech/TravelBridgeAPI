using System.Text.Json;
using TravelBridgeAPI.Models.HotelModels.HotelDetails;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleHotelDetails
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;

        public HandleHotelDetails(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKeyManager)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKeyManager ?? throw new ArgumentNullException(nameof(apiKeyManager));
        }

        public async Task<Rootobject?> GetHotelDetails(
            int hotelId,
            string arrivalDate,
            string departureDate,
            int adults,
            string? childrenAge,
            int roomQty,
            string units,
            string temperatureUnit,
            string languageCode,
            string currencyCode)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];

            string url = $"https://{apiHost}/api/v1/hotels/getHotelDetails?" +
                $"hotel_id={hotelId}" +
                $"&arrival_date={arrivalDate}" +
                $"&departure_date={departureDate}" +
                $"&adults={adults}" +
                $"&room_qty={roomQty}" +
                $"&units={units}" +
                $"&temperature_unit={temperatureUnit}" +
                $"&languagecode={languageCode}" +
                $"&currency_code={currencyCode}";

            if (!string.IsNullOrEmpty(childrenAge))
            {
                url += $"&children_age={childrenAge}";
            }

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
                Console.WriteLine($"Error: {response.StatusCode}");
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Rootobject>(jsonResponse);
        }
    }

}

