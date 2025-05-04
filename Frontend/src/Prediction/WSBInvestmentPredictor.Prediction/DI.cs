using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;

namespace WSBInvestmentPredictor.Prediction;

public static class DI
{
    /// <summary>
    /// Used to register module assembly in Routes.razor
    /// </summary>
    public static Assembly Assembly => typeof(DI).Assembly;

    public static void RegisterNavigation(IServiceProvider services)
    {
        var registry = services.GetRequiredService<NavigationRegistry>();
        registry.Links.Add(new NavLinkItem("📈 Prediction", "/predict"));
    }

}
