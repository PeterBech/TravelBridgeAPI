using System.Text.Json;
using TravelBridgeAPI.Models.HotelModels.HotelDetails;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleHotelDetails
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly ILogger<HandleHotelDetails> _logger;
        private int _logCount = 500;

        public HandleHotelDetails(
            HttpClient httpClient,
            IConfiguration configuration,
            ApiKeyManager apiKeyManager,
            ILogger<HandleHotelDetails> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKeyManager ?? throw new ArgumentNullException(nameof(apiKeyManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            _logCount++;
            if (_logCount == 601)
                _logCount = 500;
            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}\n" +
                $" Request started: {DateTime.Now}\n" +
                $" Fetching Hotel Details for:\n" +
                $" Hotel ID: {hotelId}\n" +
                $" Arrival:  {arrivalDate}\n" +
                $" Departure: {departureDate}\n" +
                $" Adults: {adults}\n" +
                $" Children Age: {childrenAge}\n" +
                $" Room Quantity: {roomQty}\n" +
                $" Units: {units}\n" +
                $" Temperature Unit: {temperatureUnit}\n" +
                $" Language Code: {languageCode}\n" +
                $" Currency Code: {currencyCode}.");


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
                        $" Failed to fetch hotel details - Status: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var hotelDetails = JsonSerializer.Deserialize<Rootobject>(jsonResponse);

                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}\n" +
                    $" Request ended: {DateTime.Now}\n" +
                    $" Successfully fetched hotel details for hotel ID: {hotelId}");

                return hotelDetails;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    $"[LOG] Log num: {_logCount}\n" +
                    $" Timestamp: {DateTime.Now}\n" +
                    $" Error fetching hotel details: {ex.Message}");
                throw;
            }
        }
    }
}

