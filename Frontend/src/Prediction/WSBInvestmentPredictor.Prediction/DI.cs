using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;

namespace WSBInvestmentPredictor.Prediction;

public static class DI
{
    /// <summary>
    /// Used to register module assembly in Routes.razor
    /// </summary>
    public static Assembly Assembly => typeof(DI).Assembly;

    // Optional: here you can later add module-wide service registration
    public static void RegisterServices(IServiceCollection services)
    {
        // services.AddScoped<IMyBlazorApp1Service, MyService>();
    }

    public static void RegisterNavigation()
    {
        NavigationRegistry.Links.Add(new NavLinkItem("📈 Prediction", "/predict"));
    }

}
