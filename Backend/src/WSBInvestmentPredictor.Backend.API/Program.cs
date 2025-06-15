using WSBInvestmentPredictor.Backend.API.Extensions;

var builder = WebApplication.CreateBuilder(args).ConfigureApplicationServices();

// Serwisy z modułów
builder.Services.ConfigureModuleServices(builder.Configuration);

var app = await builder.Build().ConfigureApplicationModules();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();
