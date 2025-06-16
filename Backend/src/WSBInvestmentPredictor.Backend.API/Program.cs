using WSBInvestmentPredictor.Backend.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Dodaj zmienne środowiskowe
builder.Configuration.AddEnvironmentVariables();

// Debug: wyświetl wartość klucza API
var apiKey = builder.Configuration["Polygon:ApiKey"];
Console.WriteLine($"Polygon API Key: {apiKey}");

builder.ConfigureApplicationServices();

// Serwisy z modułów
builder.Services.ConfigureModuleServices(builder.Configuration);

var app = await builder.Build().ConfigureApplicationModules();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();

public partial class Program
{ }