using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Reflection;
using WSBInvestmentPredictor.Expenses.Services;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Frontend.Shared.Navigation;

namespace WSBInvestmentPredictor.Expenses;

public static class DI
{
    private static IStringLocalizer<SharedResource>? _localizer;

    /// <summary>
    /// Used to register module assembly in Routes.razor
    /// </summary>
    public static Assembly Assembly => typeof(DI).Assembly;

    public static void RegisterNavigation(NavigationRegistry registry, IStringLocalizer<SharedResource> localizer)
    {
        registry.Links.Add(new NavLinkGroup(localizer["NavigationPersonalFinance"], "bi bi-wallet2")
        {
            Items =
            [
                new (localizer["NavigationExpenses"], "/expenses", "bi bi-cash"),
                new (localizer["NavigationBudget"], "/expenses/budget", "bi bi-piggy-bank"),
                new (localizer["NavigationCategories"], "/expenses/categories", "bi bi-tags"),
                new (localizer["NavigationReports"], "/expenses/reports", "bi bi-graph-up"),
                new (localizer["NavigationImportTransactions"], "/import", "bi bi-upload"),
                new (localizer["NavigationTransactions"], "/transactions", "bi bi-list-ul")
            ]
        });
    }

    public static IServiceCollection AddExpensesServices(this IServiceCollection services)
    {
        services.AddScoped<IBankTransactionService, BankTransactionService>();
        services.AddScoped<ITransactionStore, TransactionStore>();
        return services;
    }
}