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
        registry.Links.Add(new NavLinkItem("Predykcja", "/predict", "bi bi-graph-up"));
        registry.Links.Add(new NavLinkItem("Szybka predykcja", "/quick-predict", "bi bi-lightning-charge"));
        registry.Links.Add(new NavLinkItem("Backtest", "/backtest", "bi bi-arrow-repeat"));
    }
}