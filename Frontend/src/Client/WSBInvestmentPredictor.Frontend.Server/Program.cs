using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using WSBInvestmentPredictor.Frontend.Server.Components;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;
using WSBInvestmentPredictor.Frontend.Wasm;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddRadzenComponents()
    .AddRadzenCookieThemeService();

builder.Services.AddSingleton<NavigationRegistry>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    WSBInvestmentPredictor.Prediction.DI.RegisterNavigation(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([.. Routes.AdditionalAssemblies]);


await app.RunAsync();
