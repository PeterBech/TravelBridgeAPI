using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using TravelBridgeAPI.Data;
using TravelBridgeAPI.Models.FlightModels.FlightLocations;

namespace TravelBridgeAPI.DataHandlers.FlightHandlers
{
    public class HandleLocations
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly FlightLocationsContext _context;
        private readonly ILogger<HandleLocations> _logger;

        private int _logCount = 300;
        public HandleLocations(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ApiKeyManager apiKey, 
            FlightLocationsContext db,
            ILogger<HandleLocations> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _context = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Rootobject?> GetLocationAsync(string city, string language)
        {
            _logCount++;
            if (_logCount == 400)
            {
                _logCount = 300; // Resetting logcount after 300 logs
            }

            _logger.LogInformation("Fetching location started {@LocationRequestInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                City = city,
                Language = language
            });

            // Check if the database context and DbSet are initialized
            if (_context == null || _context.Rootobjects == null)
            {
                _logger.LogError(
                    $"[ERROR] Timestamp: {DateTime.Now}" +
                    $" - Database context or Rootobjects DbSet is not initialized for TravelBridgeFlightLocationDb.");
                throw new InvalidOperationException("[ERROR] Database context or Rootobjects DbSet is not initialized.");
            }



            // Check if the location is already cached in the database
            _logger.LogInformation(
                $"[LOG] Log num: {_logCount}" +
                $" Timestamp: {DateTime.Now}" +
                $" - Checking if location is already cached in the database.");
            var cachecLocation = await _context.Rootobjects
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Keyword.ToLower() == city.ToLower() && r.Language.ToLower() == language.ToLower());

            if (cachecLocation != null)
            {
                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}" +
                    $" Timestamp: {DateTime.Now}" +
                    $" - Location found in cache.");
                List<Datum> data = new List<Datum>();
                int i = 0;
                foreach (var item in _context.Data)
                {
                    if (item.Keyword.ToLower() == city.ToLower() && item.Language.ToLower() == language.ToLower())
                    {
                        i++;
                        _logger.LogInformation(
                            $"[LOG] Log num: {_logCount}" +
                            $" - Adding data object for Rootobject " +
                            $"Keyword: {city} " +
                            $"and Language: {language}.");
                        data.Add(item);
                    }
                }
                i = 0;
                foreach (var item in data)
                {
                    var distanceToCity = await _context.DistancesToCity
                        .AsNoTracking()
                        .FirstOrDefaultAsync(d => d.DatumId == item.DataId);
                    if (distanceToCity != null)
                    {
                        _logger.LogInformation(
                            $"[LOG] Log num: {_logCount}" +
                            $" - Adding DistanceToCity object for data object.");
                        item.distanceToCity = distanceToCity;
                    }
                }
                cachecLocation.data = data;
            }


            if (cachecLocation != null)
            {
                _logger.LogInformation(
                    $"[LOG] Log num: {_logCount}" +
                    $" Request ended: {DateTime.Now}" +
                    $" - Successfully fetched flight location for " +
                    $"city: {city}" +
                    $" and language: {language} from db.");
                return cachecLocation;
            }

            var newLocation = await searchLocationAsync(city, language);

            if (newLocation != null)
            {

                // Set the keyword to the city name
                newLocation.Keyword = city.ToLower();
                if (language == null)
                {
                    _logger.LogInformation(
                        $"[LOG] Log num: {_logCount}" +
                        $" Timestamp: {DateTime.Now}" +
                        $" - Adding new location to the database with" +
                        $" Keyword: {city.ToLower()} " +
                        $"and Language: en-gb.");
                    newLocation.Language = "en-gb";
                }
                else
                {
                    _logger.LogInformation(
                        $"[LOG] Log num: {_logCount}" +
                        $" Timestamp: {DateTime.Now}" +
                        $" - Adding new location to the database with" +
                        $" Keyword: {city.ToLower()}" +
                        $" and Language: {language}.");
                    newLocation.Language = language.ToLower();
                }

                // Save the new location to the database
                _context.Rootobjects.Add(newLocation);
                await _context.SaveChangesAsync();
            }
            _logger.LogInformation("Fetching location completed {@LocationSuccessInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                City = city,
                Language = language
            });

            return newLocation;
        }

        private async Task<Rootobject?> searchLocationAsync(string query, string language)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string languageCode = string.IsNullOrWhiteSpace(language) ? "en-gb" : language;
            string url = $"https://{apiHost}/api/v1/flights/searchDestination?query={query}&languagecode={languageCode}";

            _logger.LogInformation("Calling external location API {@ExternalLocationApiCallInfo}", new
            {
                LogNumber = _logCount,
                Timestamp = DateTime.UtcNow,
                Url = url
            });

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
                var flightLocations = JsonSerializer.Deserialize<Rootobject>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return flightLocations;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling external location API {@ExternalLocationApiErrorInfo}", new
                {
                    LogNumber = _logCount,
                    Timestamp = DateTime.UtcNow,
                    Url = url
                });
                return null;
            }
        }
    }
}


