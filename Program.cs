using TravelBridgeAPI;
using TravelBridgeAPI.CustomAttributes;
using TravelBridgeAPI.Data;
using TravelBridgeAPI.DataHandlers.HotelHandlers;
using TravelBridgeAPI.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using TravelBridgeAPI.DataHandlers.FlightHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key skal angives i headeren. Format: \"x-api-key: {din-n�gle}\"",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Name = "x-api-key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Tilf�j konfiguration fra appsettings.json
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>() //Henter secrets fra user-secrets
    .AddEnvironmentVariables(); // Henter env vars fra Github Actions

// Hent API-n�gler fra milj�variabler eller secrets
var apiKeys = new List<string>
{
    builder.Configuration["RapidApi:ApiKey"],
    builder.Configuration["RapidApi:ApiKey2"],
};

var baseUrl = builder.Configuration["RapidApi:BaseUrl"] ?? Environment.GetEnvironmentVariable("RapidApi:BaseUrl");

Console.WriteLine($"[INFO] BaseURL Loaded: {baseUrl}"); // Debugging log

if (string.IsNullOrEmpty(baseUrl))
{
    throw new Exception("[WARNING] RAPIDAPI_BASE_URL is missing. Check environment variables or User Secrets.");
}

// Registrer ApiKeyManager som singleton
builder.Services.AddSingleton(new ApiKeyManager(apiKeys));

//Test af ApiKeys
foreach (var key in apiKeys)
{
    Console.WriteLine($"API Key: {key}");
}
Console.WriteLine($"Base Url: {baseUrl}");

// Tilf?j HttpClient til DI-containeren
builder.Services.AddHttpClient<HandleLocations>();
builder.Services.AddScoped<HandleLocations>();

builder.Services.AddHttpClient<HandleFlightDetails>();
builder.Services.AddScoped<HandleFlightDetails>();

builder.Services.AddHttpClient<HandleFlightMinPrice>();
builder.Services.AddScoped<HandleFlightMinPrice>();

builder.Services.AddHttpClient<HandleSearch>();
builder.Services.AddScoped<HandleSearch>();

builder.Services.AddHttpClient<HandleSearchDestination>();
builder.Services.AddScoped<HandleSearchDestination>();

builder.Services.AddHttpClient<HandleSearchHotels>();
builder.Services.AddScoped<HandleSearchHotels>();

builder.Services.AddHttpClient<HandleReviewScores>();
builder.Services.AddScoped<HandleReviewScores>();

builder.Services.AddHttpClient<HandleRoomAvailability>();
builder.Services.AddScoped<HandleRoomAvailability>();

builder.Services.AddHttpClient<HandleHotelDetails>();
builder.Services.AddScoped<HandleHotelDetails>();

builder.Services.AddHttpClient<HandleHotelPhotos>();
builder.Services.AddScoped<HandleHotelPhotos>();

builder.Services.AddTransient<IApiKeyValidation, ApiKeyValidation>();
builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<FlightLocationsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("FlightLocationsContext")));


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Updates or creates the database, if it doesnt exist in the container.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FlightLocationsContext>();
    dbContext.Database.Migrate();
}

app.Run();
