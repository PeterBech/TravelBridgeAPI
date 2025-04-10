using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Text.Json;
using TravelBridgeAPI.Models.RoomAvailability;

namespace TravelBridgeAPI.DataHandlers.HotelHandlers
{
    public class HandleRoomAvailability
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;

        public HandleRoomAvailability(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
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
            if (id <= 0)
            {
                throw new ArgumentException("Hotel ID must be greater than zero.");
            }
            var rootObject = await GetRoomAvailabilityFromAPI(id, min_date, max_date, rooms, adults, currencyCode, location);
            if (rootObject != null)
            {
                return rootObject;
            }
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
