using TravelBridgeAPI;
using TravelBridgeAPI.DataHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Tilf�j konfiguration fra appsettings.json
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>() //Henter secrets fra user-secrets
    .AddEnvironmentVariables(); // Henter env vars fra Github Actions

// Hent API-n�gler fra milj�variabler eller secrets
var apiKeys = new List<string>
{
    builder.Configuration["RAPIDAPI_API_KEY"],
    builder.Configuration["RAPIDAPI_API_KEY_2"],
};

var baseUrl = builder.Configuration["RAPIDAPI_BASE_URL"] ?? Environment.GetEnvironmentVariable("RAPIDAPI_BASE_URL");

Console.WriteLine($"BaseURL Loaded: {baseUrl}"); // Debugging log

if (string.IsNullOrEmpty(baseUrl))
{
    throw new Exception("RAPIDAPI_BASE_URL is missing. Check environment variables or User Secrets.");
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

builder.Services.AddEndpointsApiExplorer();

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

app.Run();
