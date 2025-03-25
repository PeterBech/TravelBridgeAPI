using System.Text.Json;
using TravelBridgAPI.Models.FlightDetails;

namespace TravelBridgAPI.DataHandler
{
    public class HandleFlightDetails
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HandleFlightDetails(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<Rootobject?> GetFlightDetailsAsync(string token, string cc)
        {
            var rootObject = await GetFligtDetailsFromDb(token, cc);
            if (rootObject != null)
            {
                return rootObject;
            }

            rootObject = await SearchFlightDetailsAsync(token, cc);
            if (rootObject != null)
            {
                SaveFlightDetailsToDb(rootObject, cc);
                return rootObject;
            }

            return null;
        }

        private async Task<Rootobject?> GetFligtDetailsFromDb(string token, string cc)
        {
            return null;
        }

        private async Task SaveFlightDetailsToDb(Rootobject flightDetails, string cc)
        {
        }

        private async Task<Rootobject?> SearchFlightDetailsAsync(string token, string cc)
        {
            string apiKey = _configuration["RapidApi:ApiKey"];
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/flights/getFlightDetails?token={token}&currency_code={cc}";

        
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
                var flightDetails = JsonSerializer.Deserialize<Rootobject>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return flightDetails;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching flight details: {ex.Message}");
            }

        }

        
    }
}

