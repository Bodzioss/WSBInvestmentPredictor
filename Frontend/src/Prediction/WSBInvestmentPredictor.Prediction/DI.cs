using Microsoft.Extensions.Localization;
using System.Reflection;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;

namespace WSBInvestmentPredictor.Prediction;

/// <summary>
/// Dependency Injection configuration class for the Prediction module.
/// Handles module registration and navigation setup.
/// </summary>
public static class DI
{
    /// <summary>
    /// Gets the assembly containing the Prediction module components.
    /// Used to register module assembly in Routes.razor
    /// </summary>
    public static Assembly Assembly => typeof(DI).Assembly;

    /// <summary>
    /// Registers navigation items for the Prediction module.
    /// Creates a navigation group with links to prediction-related pages.
    /// </summary>
    /// <param name="registry">The navigation registry to add items to</param>
    /// <param name="localizer">The string localizer for translations</param>
    public static void RegisterNavigation(NavigationRegistry registry, IStringLocalizer<SharedResource> localizer)
    {
        registry.Links.Add(new NavLinkGroup("NavigationPredictions", "bi bi-graph-up")
        {
            Items =
            [
                new ("NavigationQuickPrediction", "/quick-predict", "bi bi-lightning-charge"),
                new("NavigationAdvancedPrediction", "/predict", "bi bi-sliders"),
                new ("NavigationBacktest", "/backtest", "bi bi-arrow-repeat")
            ]
        });
    }
}