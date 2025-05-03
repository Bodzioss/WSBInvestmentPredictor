using System.Collections.Generic;

namespace WSBInvestmentPredictor.Frontend.Shared.Navigation;

public static class NavigationRegistry
{
    public static List<NavLinkItem> Links { get; } = new();
}

public record NavLinkItem(string Label, string Url);
