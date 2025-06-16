using System.Collections.Generic;

namespace WSBInvestmentPredictor.Frontend.Shared.Navigation;

/// <summary>
/// Registry for managing navigation items in the application.
/// Maintains a collection of navigation links and groups.
/// </summary>
public class NavigationRegistry
{
    /// <summary>
    /// Collection of navigation items (links and groups) in the application.
    /// </summary>
    public List<NavLinkItem> Links { get; } = new();
}

/// <summary>
/// Represents a single navigation item in the application menu.
/// </summary>
/// <param name="Title">The display title of the navigation item</param>
/// <param name="Url">The URL the item links to</param>
/// <param name="Icon">Optional icon identifier for the item</param>
public record NavLinkItem(string Title, string Url, string? Icon = null);