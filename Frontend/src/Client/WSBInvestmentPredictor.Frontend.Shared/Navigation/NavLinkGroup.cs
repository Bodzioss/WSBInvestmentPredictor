using System.Collections.Generic;

namespace WSBInvestmentPredictor.Frontend.Shared.Navigation;

public record NavLinkGroup(string Title, string Url, string? Icon = null) : NavLinkItem(Title, Url, Icon)
{
    public List<NavLinkItem> Items { get; init; } = new();
    public bool IsExpanded { get; set; }
}