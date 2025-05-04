using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;
using WSBInvestmentPredictor.Frontend.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7214")
});

builder.Services.AddSingleton<NavigationRegistry>(provider =>
{
    var nav = new NavigationRegistry();
    nav.Links.Add(new NavLinkItem("📈 Prediction", "/predict"));
    return nav;
});

foreach (var assembly in Routes.AdditionalAssemblies)
  {
      Assembly.Load(assembly.GetName());
  }

  var host = builder.Build();

  await host.RunAsync();