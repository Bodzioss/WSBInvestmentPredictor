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
        registry.Links.Add(new NavLinkItem("📈 Prediction", "/predict"));
        registry.Links.Add(new NavLinkItem("⚡ Quick Predict", "/quick-predict"));
        registry.Links.Add(new NavLinkItem("⚡ Backtesting", "/backtest"));
    }
}