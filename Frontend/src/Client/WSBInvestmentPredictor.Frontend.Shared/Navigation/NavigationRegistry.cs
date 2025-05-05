using System.Collections.Generic;

namespace WSBInvestmentPredictor.Frontend.Shared.Navigation;

public class NavigationRegistry
{
    public List<NavLinkItem> Links { get; } = new();
}

public record NavLinkItem(string Title, string Url, string? Icon = null);