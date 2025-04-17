using System.Text.Json;
using TravelBridgeAPI.Models.FlightLocations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using TravelBridgeAPI.Data;

namespace TravelBridgeAPI.DataHandlers
{
    public class HandleLocations
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiKeyManager _apiKeyManager;
        private readonly FlightLocationsContext _db;


        public HandleLocations(HttpClient httpClient, IConfiguration configuration, ApiKeyManager apiKey, FlightLocationsContext db)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiKeyManager = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _db = db;
        }

        public async Task<Rootobject?> GetLocationAsync(string city, string language)
        {
            // Check if the location is already cached in the database
            var cachecLocation = await _db.Rootobjects
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Keyword.ToLower() == city.ToLower());

            if (cachecLocation != null)
                return cachecLocation;

            var newLocation = await searchLocationAsync(city, language);

            if (newLocation != null)
            {
                // Set the keyword to the city name
                newLocation.Keyword = city.ToLower();
                // Save the new location to the database
                _db.Rootobjects.Add(newLocation);
                await _db.SaveChangesAsync();
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

            Console.WriteLine($"SearchLocationAPI Key: {apiKey}");
            Console.WriteLine($"SearchLocationAPI BaseURL: {apiHost}");

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


public static class RootobjectEndpoints
{
	public static void MapRootobjectEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Rootobject").WithTags(nameof(Rootobject));

        group.MapGet("/", async (FlightLocationsContext db) =>
        {
            return await db.Rootobjects.ToListAsync();
        })
        .WithName("GetAllRootobjects")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Rootobject>, NotFound>> (string keyword, FlightLocationsContext db) =>
        {
            return await db.Rootobjects.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Keyword == keyword)
                is Rootobject model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetRootobjectById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (string keyword, Rootobject rootobject, FlightLocationsContext db) =>
        {
            var affected = await db.Rootobjects
                .Where(model => model.Keyword == keyword)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Keyword, rootobject.Keyword)
                  .SetProperty(m => m.status, rootobject.status)
                  .SetProperty(m => m.message, rootobject.message)
                  .SetProperty(m => m.timestamp, rootobject.timestamp)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateRootobject")
        .WithOpenApi();

        group.MapPost("/", async (Rootobject rootobject, FlightLocationsContext db) =>
        {
            db.Rootobjects.Add(rootobject);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Rootobject/{rootobject.Keyword}",rootobject);
        })
        .WithName("CreateRootobject")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (string keyword, FlightLocationsContext db) =>
        {
            var affected = await db.Rootobjects
                .Where(model => model.Keyword == keyword)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteRootobject")
        .WithOpenApi();
    }
}}
