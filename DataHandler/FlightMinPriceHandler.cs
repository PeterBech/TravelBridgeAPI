using System.Runtime.InteropServices;
using System.Text.Json;
using TravelBridgAPI.Models.FlightMinPrice;

namespace TravelBridgAPI.DataHandler
{
    public class FlightMinPriceHandler
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FlightMinPriceHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<Rootobject?> GetMinFlightPrice(string from, string to, string departure, string returnFlight, string cabinClass, string curencyCode)
        {
            var rootObject = await GetMinFlightPriceFromDb(from, to);
            if (rootObject != null)
            {
                return rootObject;
            }

            rootObject = await SearchMinFlightPrice(from, to, departure, returnFlight, cabinClass, curencyCode);
            if (rootObject != null)
            {
                SaveMinFlightPriceFromDb(rootObject);
                return rootObject;
            }

            return null;
        }

        private async Task<Rootobject?> GetMinFlightPriceFromDb(string from, string to)
        {
            return null;
        }

        private async Task SaveMinFlightPriceFromDb(Rootobject flightDetails)
        {
        }

        private async Task<Rootobject?> SearchMinFlightPrice(string from, string to, string departure, string returnFlight, string cabinClass, string curencyCode)
        {
            string apiKey = _configuration["RapidApi:ApiKey"];
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/flights/getMinPrice?fromId={from}&toId={to}&departDate={departure}&returnDate={returnFlight}&cabinClass={cabinClass}&currency_code={curencyCode}";


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
                var flightMinPrice = JsonSerializer.Deserialize<Rootobject>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return flightMinPrice;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching flight details: {ex.Message}");
            }

        }
    }
}
