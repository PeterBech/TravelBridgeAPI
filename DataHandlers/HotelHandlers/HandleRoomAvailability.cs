using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Text.Json;
using TravelBridgeAPI.Models.HotelModels.RoomAvailability;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleRoomAvailability
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly ILogger<HandleRoomAvailability> _logger;

        private int _logCount = 800;

        public HandleRoomAvailability(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ApiKeyManager apiKey, 
            ILogger<HandleRoomAvailability> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Rootobject> GetRoomAvailability(
            int id, 
            string? min_date, 
            string? max_date, 
            int? rooms, 
            int? adults, 
            string? currencyCode, 
            string? location)
        {
            _logCount++;
            if (_logCount == 901)
                _logCount = 800; // Reset the counter after 100 requests

            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}" +
                $" Request started: {DateTime.Now}" +
                $" - Fetching room availability for hotel ID: {id} " +
                $"with min_date: {min_date}, " +
                $"max_date: {max_date}.");
            if (id <= 0)
            {
                _logger.LogError($"[LOG] Log num: {_logCount} - Invalid hotel ID: {id}. Must be greater than zero.");
                throw new ArgumentException("Hotel ID must be greater than zero.");
            }
            var rootObject = await GetRoomAvailabilityFromAPI(id, min_date, max_date, rooms, adults, currencyCode, location);
            if (rootObject != null)
            {
                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}" +
                    $" Request ended: {DateTime.Now}" +
                    $" - Successfully fetched room availability for hotel ID: {id} " +
                    $"with min_date: {min_date}, " +
                    $"max_date: {max_date}.");
                return rootObject;
            }
            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}" +
                $" Request ended: {DateTime.Now}" +
                $" - No room availability found for hotel ID: {id}.");
            return null;
        }

        public async Task<Rootobject> GetRoomAvailabilityFromAPI(
            int id,
            string? min_date,
            string? max_date,
            int? rooms,
            int? adults,
            string? currencyCode,
            string? location)
        {
            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}" +
                $" Timestamp: {DateTime.Now}" +
                $" - Fetching room availability for hotel ID: {id} " +
                $"with min_date: {min_date}, " +
                $"max_date: {max_date} " +
                $"from external API.");
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/hotels/getAvailability?hotel_id={id}";
            if (!string.IsNullOrWhiteSpace(min_date))
                url += $"&min_date={min_date}";
            if (!string.IsNullOrWhiteSpace(max_date))
                url += $"&max_date={max_date}";
            if (rooms != null)
                url += $"&rooms={rooms}";
            if (adults != null)
                url += $"&adults={adults}";
            if (!string.IsNullOrWhiteSpace(currencyCode))
                url += $"&currency_code={currencyCode}";
            if (!string.IsNullOrWhiteSpace(location))
                url += $"&location=" + location;

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
                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}" +
                    $" Timestamp: {DateTime.Now}" +
                    $" - Successfully fetched room availability" +
                    $" from external API.");
                var jsonString = await response.Content.ReadAsStringAsync();
                var flightMinPrice = JsonSerializer.Deserialize<Rootobject>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return flightMinPrice;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    $"[LOG] Log num: {_logCount}" +
                    $" Timestamp: {DateTime.Now}" +
                    $" - Error fetching room availability: {ex.Message}");
                throw new Exception($"Error fetching flight details: {ex.Message}");
            }
        }
    }
}
