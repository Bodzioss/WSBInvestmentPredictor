using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.JSInterop;
using Radzen;
using System.Globalization;
using System.Reflection;
using WSBInvestmentPredictor.Expenses;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;
using WSBInvestmentPredictor.Frontend.Shared.Services;
using WSBInvestmentPredictor.Frontend.Wasm;
using WSBInvestmentPredictor.Frontend.Wasm.Services.Cqrs;
using WSBInvestmentPredictor.Technology.Cqrs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure backend URL based on environment
var backendUrl = builder.HostEnvironment.IsDevelopment()
    ? "https://localhost:7214"
    : "https://wsbinvestmentpredictor-gvgxemaubagncrb0.polandcentral-01.azurewebsites.net";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(backendUrl)
});

builder.Services.AddRadzenComponents()
    .AddRadzenCookieThemeService();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ICqrsRequestService, HttpCqrsRequestService>();
builder.Services.AddFrontendSharedServices();

// Add Expenses services
builder.Services.AddExpensesServices();

// Configure localization options
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("pl")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en"); // Default to English initially
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var nav = new NavigationRegistry();
WSBInvestmentPredictor.Prediction.DI.RegisterNavigation(nav);
WSBInvestmentPredictor.Expenses.DI.RegisterNavigation(nav);
builder.Services.AddSingleton(nav);

foreach (var assembly in Routes.AdditionalAssemblies)
{
    Assembly.Load(assembly.GetName());
}

var host = builder.Build();

// Get the culture from localStorage
var js = host.Services.GetRequiredService<IJSRuntime>();
var savedCulture = await js.InvokeAsync<string>("localStorage.getItem", "culture");

// If no culture is saved, default to English
var cultureName = !string.IsNullOrEmpty(savedCulture) ? savedCulture : "en";
var cultureInfo = new CultureInfo(cultureName);

// Set the culture
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
CultureInfo.CurrentCulture = cultureInfo;
CultureInfo.CurrentUICulture = cultureInfo;

// Save to localStorage if it wasn't already set or if it's different from the default
if (string.IsNullOrEmpty(savedCulture) || savedCulture != cultureName)
{
    await js.InvokeVoidAsync("localStorage.setItem", "culture", cultureName);
}

await host.RunAsync();