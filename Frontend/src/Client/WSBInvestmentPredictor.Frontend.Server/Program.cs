using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WSBInvestmentPredictor.Frontend.Wasm;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Load Blazor UI and pass assemblies registered in WASM
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([.. Routes.AdditionalAssemblies]);

await app.RunAsync();
