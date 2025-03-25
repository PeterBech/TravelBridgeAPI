using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TravelBridgAPI.DataHandler;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Tilføj konfiguration fra appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Tilf�j HttpClient til DI-containeren
builder.Services.AddHttpClient<HandleLocations>();
builder.Services.AddScoped<HandleLocations>();

builder.Services.AddHttpClient<HandleFlightDetails>();
builder.Services.AddScoped<HandleFlightDetails>();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
