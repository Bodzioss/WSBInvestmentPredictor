using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
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

// Create the WebAssembly host builder
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure backend URL based on environment
var backendUrl = builder.HostEnvironment.IsDevelopment()
    ? "https://localhost:7250"
    : builder.HostEnvironment.BaseAddress.Contains("dev")
        ? "https://wsbinvestmentpredictor-dev-dzafhchaa7b0hba3.polandcentral-01.azurewebsites.net" // Dev backend
        : "https://wsbinvestmentpredictor-gvgxemaubagncrb0.polandcentral-01.azurewebsites.net"; // Production backend

Console.WriteLine($"Using backend URL: {backendUrl}");

// Configure HTTP client with backend URL
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(backendUrl)
});

// Add Radzen UI components and services
builder.Services.AddRadzenComponents()
    .AddRadzenCookieThemeService();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ICqrsRequestService, HttpCqrsRequestService>();
builder.Services.AddFrontendSharedServices();

// Add Expenses module services
builder.Services.AddExpensesServices();

// Configure supported cultures for localization
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("pl")
};

// Configure localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en"); // Default to English initially
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Register navigation items from all modules
var nav = new NavigationRegistry();
var localizer = builder.Services.BuildServiceProvider().GetRequiredService<IStringLocalizer<SharedResource>>();
WSBInvestmentPredictor.Prediction.DI.RegisterNavigation(nav, localizer);
WSBInvestmentPredictor.Expenses.DI.RegisterNavigation(nav, localizer);
builder.Services.AddSingleton(nav);

// Load additional assemblies for routing
foreach (var assembly in Routes.AdditionalAssemblies)
{
    Assembly.Load(assembly.GetName());
}

// Build the host
var host = builder.Build();

// Get the culture from localStorage
var js = host.Services.GetRequiredService<IJSRuntime>();
var savedCulture = await js.InvokeAsync<string>("localStorage.getItem", "culture");

// If no culture is saved, default to English
var cultureName = !string.IsNullOrEmpty(savedCulture) ? savedCulture : "en";
var cultureInfo = new CultureInfo(cultureName);

// Set the culture for the application
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
CultureInfo.CurrentCulture = cultureInfo;
CultureInfo.CurrentUICulture = cultureInfo;

// Save to localStorage if it wasn't already set or if it's different from the default
if (string.IsNullOrEmpty(savedCulture) || savedCulture != cultureName)
{
    await js.InvokeVoidAsync("localStorage.setItem", "culture", cultureName);
}

// Run the application
await host.RunAsync();