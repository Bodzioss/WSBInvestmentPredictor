using System.Collections.Generic;

namespace WSBInvestmentPredictor.Frontend.Shared.Navigation;

/// <summary>
/// Represents a group of navigation links in the sidebar menu.
/// Extends NavLinkItem to support nested navigation items.
/// </summary>
/// <param name="Title">The display title of the navigation group</param>
/// <param name="Url">The URL associated with the group (if any)</param>
/// <param name="Icon">Optional icon identifier for the group</param>
public record NavLinkGroup(string Title, string Url, string? Icon = null) : NavLinkItem(Title, Url, Icon)
{
    /// <summary>
    /// List of navigation items contained within this group.
    /// </summary>
    public List<NavLinkItem> Items { get; init; } = new();

    /// <summary>
    /// Indicates whether the navigation group is expanded in the UI.
    /// </summary>
    public bool IsExpanded { get; set; }
}