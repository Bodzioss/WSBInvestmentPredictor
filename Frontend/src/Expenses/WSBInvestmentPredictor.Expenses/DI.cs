using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WSBInvestmentPredictor.Expenses.Services;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;

namespace WSBInvestmentPredictor.Expenses;

public static class DI
{
    /// <summary>
    /// Used to register module assembly in Routes.razor
    /// </summary>
    public static Assembly Assembly => typeof(DI).Assembly;

    public static void RegisterNavigation(NavigationRegistry registry)
    {
        registry.Links.Add(new NavLinkGroup("Finanse Osobiste", "bi bi-wallet2")
        {
            Items = new List<NavLinkItem>
            {
                new NavLinkItem("Wydatki", "/expenses", "bi bi-cash"),
                new NavLinkItem("Budżet", "/expenses/budget", "bi bi-piggy-bank"),
                new NavLinkItem("Kategorie", "/expenses/categories", "bi bi-tags"),
                new NavLinkItem("Raporty", "/expenses/reports", "bi bi-graph-up"),
                new NavLinkItem("Import Transakcji", "/import", "bi bi-upload"),
                new NavLinkItem("Transakcje", "/transactions", "bi bi-list-ul")
            }
        });
    }

    public static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IBankTransactionService, BankTransactionService>();
        services.AddSingleton<ITransactionStore, TransactionStore>();
    }
}