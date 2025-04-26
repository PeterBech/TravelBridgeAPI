using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json;
using TravelBridgeAPI.Models.FlightModels.FlightSearches;
using TravelBridgeAPI.Security;

namespace TravelBridgeAPI.DataHandlers.FlightHandlers
{
    public class HandleSearch
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IApiKeyValidation _apiKeyValidation;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly ILogger<HandleSearch> _logger;

        private int _logCount = 400;

        public HandleSearch(
            HttpClient httpClient, 
            IConfiguration configuration, 
            HandleLocations handleLocations,
            ApiKeyManager apiKey, 
            ILogger<HandleSearch> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _logger = logger;
        }

        public async Task<Rootobject?> GetDirectFlightAsync(
            string fromId,
            string toId,
            string date,
            string? sort = null,
            string? cabinClass = null,
            string? currency = null)
        {
            _logCount++;
            if (_logCount == 501)
                _logCount = 400; // Reset the counter after 100 requests

            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}" +
                $" Request started: {DateTime.Now}" +
                $" - Fetching direct flights with" +
                $" departure location: {fromId}" +
                $" arrival location: {toId}" +
                $" on {date}" +
                $" from external API.");

            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];

            string url = $"https://{apiHost}/api/v1/flights/searchFlights" +
                         $"?fromId={fromId}" +
                         $"&toId={toId}" +
                         $"&departDate={date}" +
                         $"&pageNo=1" +
                         $"&adults=1";

            if (!string.IsNullOrEmpty(sort)) url += $"&sort={sort}";
            if (!string.IsNullOrEmpty(cabinClass)) url += $"&cabinClass={cabinClass}";
            if (!string.IsNullOrEmpty(currency)) url += $"&currency_code={currency}";


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

            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                var flightRoutes = JsonSerializer.Deserialize<Rootobject?>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}" +
                    $" Request ended: {DateTime.Now}" +
                    $" - Successfully fetched direct flights with" +
                    $" departure location: {fromId}" +
                    $" arrival location: {toId}" +
                    $" on {date}.");
                return flightRoutes;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    $"[LOG] Log num: {_logCount}" +
                    $" Timestamp: {DateTime.Now}" +
                    $" - Error fetching direct flights: {ex.Message}."); 
                return null;
            }
        }
    }
}
