using System.Collections.Generic;

namespace WSBInvestmentPredictor.Frontend.Shared.Navigation;

/// <summary>
/// Represents a group of navigation links in the sidebar menu.
/// Extends NavLinkItem to support nested navigation items.
/// </summary>
/// <param name="TitleKey">The localization key for the navigation group title</param>
/// <param name="Url">The URL associated with the group (if any)</param>
/// <param name="Icon">Optional icon identifier for the group</param>
public record NavLinkGroup(string TitleKey, string Url, string? Icon = null) : NavLinkItem(TitleKey, Url, Icon)
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