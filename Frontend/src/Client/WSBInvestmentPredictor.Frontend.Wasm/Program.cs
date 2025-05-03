using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WSBInvestmentPredictor.Frontend.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

  foreach (var assembly in Routes.AdditionalAssemblies)
  {
      Assembly.Load(assembly.GetName());
  }

  var host = builder.Build();

  await host.RunAsync();