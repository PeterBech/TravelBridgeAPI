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

            _logger.LogInformation("Fetching hotel details started {@HotelDetailsRequestInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                HotelId = hotelId,
                ArrivalDate = arrivalDate,
                DepartureDate = departureDate,
                Adults = adults,
                ChildrenAge = childrenAge,
                RoomQty = roomQty,
                Units = units,
                TemperatureUnit = temperatureUnit,
                LanguageCode = languageCode,
                CurrencyCode = currencyCode
            });


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
                    _logger.LogWarning("Failed to fetch hotel details {@HotelDetailsWarningInfo}", new
                    {
                        LogNumber = _logCount,
                        Timestamp = DateTime.UtcNow,
                        StatusCode = response.StatusCode
                    });
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var hotelDetails = JsonSerializer.Deserialize<Rootobject>(jsonResponse);

                _logger.LogInformation("Successfully fetched hotel details {@HotelDetailsSuccessInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    HotelId = hotelId
                });

                return hotelDetails;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching hotel details {@HotelDetailsErrorInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    HotelId = hotelId
                });
                throw;
            }
        }
    }
}

