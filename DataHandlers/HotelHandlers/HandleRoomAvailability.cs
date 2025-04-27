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

        public async Task<Rootobject?> GetRoomAvailability(
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

            _logger.LogInformation("Fetching room availability started {@RoomAvailabilityRequestInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                HotelId = id,
                MinDate = min_date,
                MaxDate = max_date
            });

            if (id <= 0)
            {
                _logger.LogError("Invalid hotel ID provided {@RoomAvailabilityErrorInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    HotelId = id,
                    MinDate = min_date,
                    MaxDate = max_date
                });
                throw new ArgumentException("Hotel ID must be greater than zero.");
            }

            var rootObject = await GetRoomAvailabilityFromAPI(id, min_date, max_date, rooms, adults, currencyCode, location);

            if (rootObject != null)
            {
                _logger.LogInformation("Successfully fetched room availability {@RoomAvailabilitySuccessInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    HotelId = id
                });
                return rootObject;
            }
            _logger.LogWarning("No room availability found {@RoomAvailabilityWarningInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                HotelId = id
            });
            return null;
        }

        private async Task<Rootobject?> GetRoomAvailabilityFromAPI(
            int id,
            string? minDate,
            string? maxDate,
            int? rooms,
            int? adults,
            string? currencyCode,
            string? location)

        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];

            var queryParams = new List<string> { $"hotel_id={id}" };
            if (!string.IsNullOrWhiteSpace(minDate)) queryParams.Add($"min_date={minDate}");
            if (!string.IsNullOrWhiteSpace(maxDate)) queryParams.Add($"max_date={maxDate}");
            if (rooms != null) queryParams.Add($"rooms={rooms}");
            if (adults != null) queryParams.Add($"adults={adults}");
            if (!string.IsNullOrWhiteSpace(currencyCode)) queryParams.Add($"currency_code={currencyCode}");
            if (!string.IsNullOrWhiteSpace(location)) queryParams.Add($"location={location}");

            string url = $"https://{apiHost}/api/v1/hotels/getAvailability?" + string.Join("&", queryParams);

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                    Headers =
                    {
                        { "x-rapidapi-key", apiKey },
                        { "x-rapidapi-host", apiHost }
                    }
                };

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var roomAvailability = JsonSerializer.Deserialize<Rootobject>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return roomAvailability;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching room availability {@RoomAvailabilityApiErrorInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    HotelId = id
                });
                throw;
            }
        }
    }
}