using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Globalization;
using System.Text;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Expenses.Services;
using WSBInvestmentPredictor.Technology.Cqrs;
using Radzen;

namespace WSBInvestmentPredictor.Expenses
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Register encoding provider for Windows-1250
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<WSBInvestmentPredictor.Expenses.App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Add shared services including localization
            builder.Services.AddFrontendSharedServices();
            
            // Add Radzen services
            builder.Services.AddRadzenComponents()
                .AddRadzenCookieThemeService();
            builder.Services.AddScoped<NotificationService>();

            // Add application services
            builder.Services.AddScoped<IBankTransactionService, BankTransactionService>();
            builder.Services.AddScoped<ITransactionStore, TransactionStore>();

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
        }
    }
}
