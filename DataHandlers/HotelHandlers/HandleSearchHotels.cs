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

            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}" +
                $" Request started: {DateTime.Now}" +
                $" - Fetching hotel search for destination ID: {dest_id}," +
                $" search type: {search_type}," +
                $" arrival date: {arrival}," +
                $" departure date: {departure}.");
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
                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}" +
                    $" Request ended: {DateTime.Now}" +
                    $" - Successfully fetched hotel search.");
                return hotel;
            }
            _logger.LogWarning(
                $"[LOG] Log num: {_logCount}" +
                $" Request ended: {DateTime.Now}" +
                $" - No hotel search results found.");
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
            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}" +
                $" Timestamp: {DateTime.Now}" +
                $" - Fetching hotel search for destination ID: {dest_id}," +
                $" search type: {search_type}," +
                $" arrival date: {arrival}," +
                $" departure date: {departure}" +
                $" from external API.");
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/hotels/searchHotels?dest_id={dest_id}&search_type={search_type}&arrival_date={arrival}&departure_date={departure}";

            if (adults != null)
            {
                url += $"&adults={adults}";
            }
            if (children != null)
            {
                url += $"&children_age={children}";
            }
            if (room_qty != null)
            {
                url += $"&room_qty={room_qty}";
            }
            if (page_number != null)
            {
                url += $"&page_number={page_number}";
            }
            if (minPrice != null)
            {
                url += $"&price_min={minPrice}";
            }
            if (maxPrice != null)
            {
                url += $"&price_max={maxPrice}";
            }
            if (units != null)
            {
                url += $"&units={units}";
            }
            if (tempUnit != null)
            {
                url += $"&temperature_unit={tempUnit}";
            }
            if (language != null)
            {
                url += $"&languagecode={language}";
            }
            if (currencyCode != null)
            {
                url += $"&currency_code={currencyCode}";
            }
            if (location != null)
            {
                url += $"&location={location}";
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

            try
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation(
                            $"[LOG] Log num: {_logCount}" +
                            $" Timestamp: {DateTime.Now}" +
                            $" - Successfully fetched hotel search.");
                        return JsonSerializer.Deserialize<Rootobject>(jsonResponse);
                    }
                    else
                    {
                        _logger.LogError(
                            $"[LOG] Log num: {_logCount}" +
                            $" Timestamp: {DateTime.Now}" +
                            $" - Error fetching hotel search: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"[LOG] Log num: {_logCount}" +
                    $" Timestamp: {DateTime.Now}" +
                    $" - Exception occurred while fetching hotel search: {ex.Message}");
                return null;
            }
        }
    }
}
