using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using WSBInvestmentPredictor.Frontend.Server.Components;
using WSBInvestmentPredictor.Frontend.Wasm;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddRadzenComponents()
    .AddRadzenCookieThemeService();

builder.Services.AddScoped<NotificationService>();

var app = builder.Build();

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