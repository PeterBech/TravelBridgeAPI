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


        public HandleLocations(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKey, FlightLocationsContext db)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _context = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Rootobject?> GetLocationAsync(string city, string language)
        {

            if (_context == null || _context.Rootobjects == null)
            {
                throw new InvalidOperationException("[ERROR] Database context or Rootobjects DbSet is not initialized.");
            }

            // Check if the location is already cached in the database
            var cachecLocation = await _context.Rootobjects
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Keyword.ToLower() == city.ToLower() && r.Language.ToLower() == language.ToLower());
            // implementer logik til at hente Datum ud fra keyword og language, og gemme i en ny liste
            // med dertilhørende DistanceToCity
            if(cachecLocation != null)
            {
                List<Datum> data = new List<Datum>();
                foreach(var item in _context.Data)
                {
                    if(item.Keyword.ToLower() == city.ToLower() && item.Language.ToLower() == language.ToLower())
                    {
                        data.Add(item);
                    }
                }
                foreach (var item in data)
                {
                    var distanceToCity = await _context.DistancesToCity
                        .AsNoTracking()
                        .FirstOrDefaultAsync(d => d.DatumId == item.DataId);
                    if (distanceToCity != null)
                    {
                        item.distanceToCity = distanceToCity;
                    }
                }
                cachecLocation.data = data;
            }


            if (cachecLocation != null)
                return cachecLocation;

            var newLocation = await searchLocationAsync(city, language);

            if (newLocation != null)
            {
                // Set the keyword to the city name
                newLocation.Keyword = city.ToLower();
                if(language == null)
                {
                    newLocation.Language = "en-gb";
                }
                else
                {
                    newLocation.Language = language.ToLower();
                }
                // Save the new location to the database
                _context.Rootobjects.Add(newLocation);
                await _context.SaveChangesAsync();
            }
            return newLocation;
        }
        //public async Task<Rootobject?> GetLocationAsync(string city, string language)
        //{
        //    var flightLocation = await searchLocationAsync(city, language);
        //    if (flightLocation != null)
        //    {
        //        return flightLocation;
        //    }

        //    return null;
        //}

        private async Task<Rootobject?> searchLocationAsync(string query, string language)
        {
            string apiKey = _apiKeyManager.GetNextApiKey();
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string languageCode = string.IsNullOrWhiteSpace(language) ? "en-gb" : language;
            string url = $"https://{apiHost}/api/v1/flights/searchDestination?query={query}&languagecode={languageCode}";

            Console.WriteLine($"[INFO] SearchLocationAPI Key: {apiKey}");
            Console.WriteLine($"[INFO] SearchLocationAPI BaseURL: {apiHost}");

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
                var flightLocations = JsonSerializer.Deserialize<Rootobject>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return flightLocations;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[ERROR] Fetching flight destination failed: {ex.Message}");
                return null;
            }
        }
    }
}


