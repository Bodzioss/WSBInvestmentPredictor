using WSBInvestmentPredictor.Backend.API.Extensions;
using WSBInvestmentPredictor.Backend.API.Cqrs;

// Add environment variables
var builder = WebApplication.CreateBuilder(args);

// Debug: display API key value
Console.WriteLine($"API Key: {builder.Configuration["PolygonApiKey"]}");

builder.ConfigureApplicationServices();
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