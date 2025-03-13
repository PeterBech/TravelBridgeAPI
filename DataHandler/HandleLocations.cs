using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using TravelBridgAPI.Models.FlightLocations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using TravelBridgAPI.Data;

namespace TravelBridgAPI.DataHandler
{
    public class HandleLocations
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly FlightLocationsContext _dbContext;

        public HandleLocations(HttpClient httpClient, IConfiguration configuration, FlightLocationsContext dbContext)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dbContext = dbContext;
        }

        public async Task<Rootobject?> GetLocationAsync(string city)
        {
            var rootObject = await GetLocationFromDb(city);
            if (rootObject != null)
            {
                return rootObject;
            }

            rootObject = await SearchLocationAsync(city);
            if (rootObject != null)
            {
                SaveLocationToDb(rootObject);
                return rootObject;
            }

            return null;
        }

        private async Task<Rootobject?> GetLocationFromDb(string city)
        {
            //return await _dbContext.Rootobject
            //    .Include(r => r.data) // Henter relaterede `Datum`-data
            //    .ThenInclude(d => d.DistanceToCity) // Henter `Distancetocity`-relationen
            //    .FirstOrDefaultAsync(r => r.Keyword == city);
            return null;
        }

        private async Task SaveLocationToDb(Rootobject location)
        {
            //_dbContext.Rootobject.Add(location);
            //await _dbContext.SaveChangesAsync();
        }

        private async Task<Rootobject?> SearchLocationAsync(string query)
        {
            string apiKey = _configuration["RapidApi:ApiKey"];
            string apiHost = _configuration["RapidApi:BaseUrl"];
            string url = $"https://{apiHost}/api/v1/flights/searchDestination?query={query}";

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
                Rootobject locations = flightLocations;
                locations.Keyword = query;
                return locations;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching flight destination: {ex.Message}");
            }
        }
    }
    public static class RootobjectEndpoints
    {
        public static void MapRootobjectEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Rootobject").WithTags(nameof(Rootobject));

            group.MapGet("/{city}", async Task<Results<Ok<Rootobject>, NotFound>> (string city, HandleLocations handler) =>
            {
                var location = await handler.GetLocationAsync(city);
                return location is not null
                    ? TypedResults.Ok(location)
                    : TypedResults.NotFound();
            })
            .WithName("GetLocation")
            .WithOpenApi();
        }
    }
}

//public static class RootobjectEndpoints
//{
//	public static void MapRootobjectEndpoints (this IEndpointRouteBuilder routes)
//    {
//        var group = routes.MapGroup("/api/Rootobject").WithTags(nameof(Rootobject));

//        group.MapGet("/", async (FlightLocationsContext db) =>
//        {
//            return await db.Rootobject.ToListAsync();
//        })
//        .WithName("GetAllRootobjects")
//        .WithOpenApi();

//        group.MapGet("/{id}", async Task<Results<Ok<Rootobject>, NotFound>> (string keyword, FlightLocationsContext db) =>
//        {
//            return await db.Rootobject.AsNoTracking()
//                .FirstOrDefaultAsync(model => model.Keyword == keyword)
//                is Rootobject model
//                    ? TypedResults.Ok(model)
//                    : TypedResults.NotFound();
//        })
//        .WithName("GetRootobjectById")
//        .WithOpenApi();

//        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (string keyword, Rootobject rootobject, FlightLocationsContext db) =>
//        {
//            var affected = await db.Rootobject
//                .Where(model => model.Keyword == keyword)
//                .ExecuteUpdateAsync(setters => setters
//                  .SetProperty(m => m.Keyword, rootobject.Keyword)
//                  .SetProperty(m => m.status, rootobject.status)
//                  .SetProperty(m => m.message, rootobject.message)
//                  .SetProperty(m => m.timestamp, rootobject.timestamp)
//                  );
//            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
//        })
//        .WithName("UpdateRootobject")
//        .WithOpenApi();

//        group.MapPost("/", async (Rootobject rootobject, FlightLocationsContext db) =>
//        {
//            db.Rootobject.Add(rootobject);
//            await db.SaveChangesAsync();
//            return TypedResults.Created($"/api/Rootobject/{rootobject.Keyword}",rootobject);
//        })
//        .WithName("CreateRootobject")
//        .WithOpenApi();

//        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (string keyword, FlightLocationsContext db) =>
//        {
//            var affected = await db.Rootobject
//                .Where(model => model.Keyword == keyword)
//                .ExecuteDeleteAsync();
//            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
//        })
//        .WithName("DeleteRootobject")
//        .WithOpenApi();
//    }
//}}

