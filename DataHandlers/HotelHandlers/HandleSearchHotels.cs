using NuGet.Packaging.Signing;
using System.Net.Http;
using System.Text.Json;
using TravelBridgeAPI.Models.HotelModels;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleSearchHotels
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly ILogger<HandleSearchHotels> _logger;

        private int _logCount = 1000;

        public HandleSearchHotels(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ApiKeyManager apiKeyManager, 
            ILogger<HandleSearchHotels> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKeyManager ?? throw new ArgumentNullException(nameof(apiKeyManager)); ;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        }

        public async Task<Rootobject?> GetHotel(
            string dest_id,
            string search_type,
            string arrival,
            string departure,
            string? adults,
            string? children,
            int? room_qty,
            int? page_number,
            int? minPrice,
            int? maxPrice,
            string? units,
            string? tempUnit,
            string? language,
            string? currencyCode,
            string? location)
        {
            _logCount++;
            if (_logCount == 1101)
            {
                _logCount = 1000; // Resetting logcount after 100 logs
            }

            _logger.LogInformation("Fetching hotel search started {@HotelSearchRequestInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                DestinationId = dest_id,
                SearchType = search_type,
            });

            var hotel = await SearchHotelAsync(
                dest_id, 
                search_type,
                arrival,
                departure,
                adults,
                children,
                room_qty,
                page_number,
                minPrice,
                maxPrice,
                units,
                tempUnit,
                language,
                currencyCode,
                location);
   
            if (hotel != null)
            {
                _logger.LogInformation("Successfully fetched hotel search {@HotelSearchSuccessInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    DestinationId = dest_id
                });
                return hotel;
            }

            _logger.LogWarning("No hotel search found {@HotelSearchWarningInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                DestinationId = dest_id
            });
            
            return null;
        }

        private async Task<Rootobject?> SearchHotelAsync(
            string dest_id,
            string search_type,
            string arrival,
            string departure,
            string? adults,
            string? children,
            int? room_qty,
            int? page_number,
            int? minPrice,
            int? maxPrice,
            string? units,
            string? tempUnit,
            string? language,
            string? currencyCode,
            string? location)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            var queryParams = new List<string>
            {
                $"dest_id={dest_id}",
                $"search_type={search_type}",
                $"arrival_date={arrival}",
                $"departure_date={departure}"
            };
            
            if (adults != null) queryParams.Add($"adults={adults}");
            if (children != null) queryParams.Add($"children_age={children}");
            if (room_qty != null) queryParams.Add($"room_qty={room_qty}");
            if (page_number != null) queryParams.Add($"page_number={page_number}");
            if (minPrice != null) queryParams.Add($"price_min={minPrice}");
            if (maxPrice != null) queryParams.Add($"price_max={maxPrice}");
            if (units != null) queryParams.Add($"units={units}");
            if (tempUnit != null) queryParams.Add($"temperature_unit={tempUnit}");
            if (language != null) queryParams.Add($"languagecode={language}");
            if (currencyCode != null) queryParams.Add($"currency_code={currencyCode}");
            if (location != null) queryParams.Add($"location={location}");

            string url = $"https://{apiHost}/api/v1/hotels/searchHotels?dest_id={dest_id}&search_type={search_type}&arrival_date={arrival}&departure_date={departure}";

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

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Rootobject>(jsonString);
                }
                else
                {
                    _logger.LogError("Error fetching hotel search {@HotelSearchApiErrorInfo}", new
                    {
                        LogNumber = _logCount,
                        Timestamp = DateTime.UtcNow,
                        StatusCode = response.StatusCode
                    });
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception fetching hotel search {@HotelSearchExceptionInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow
                });
                return null;
            }
        }
    }
}