using System.Collections.Generic;
using System.Reflection;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;

namespace WSBInvestmentPredictor.Prediction;

public static class DI
{
    /// <summary>
    /// Used to register module assembly in Routes.razor
    /// </summary>
    public static Assembly Assembly => typeof(DI).Assembly;

    public static void RegisterNavigation(NavigationRegistry registry)
    {
        registry.Links.Add(new NavLinkGroup("Predykcje", "bi bi-graph-up")
        {
            Items = new List<NavLinkItem>
            {
                new NavLinkItem("Szybka predykcja", "/quick-predict", "bi bi-lightning-charge"),
                new NavLinkItem("Zaawansowana predykcja", "/predict", "bi bi-sliders"),
                new NavLinkItem("Backtest", "/backtest", "bi bi-arrow-repeat")
            }
        });
    }
}