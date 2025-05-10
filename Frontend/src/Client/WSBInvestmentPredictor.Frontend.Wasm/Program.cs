using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using System.Reflection;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;
using WSBInvestmentPredictor.Frontend.Shared.Services;
using WSBInvestmentPredictor.Frontend.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7214")
});

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ApiService>();

var nav = new NavigationRegistry();
WSBInvestmentPredictor.Prediction.DI.RegisterNavigation(nav);
builder.Services.AddSingleton(nav);

foreach (var assembly in Routes.AdditionalAssemblies)
{
    Assembly.Load(assembly.GetName());
}

var host = builder.Build();

await host.RunAsync();