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
        registry.Links.Add(new NavLinkGroup("NavigationPersonalFinance", "bi bi-wallet2")
        {
            Items =
            [
                new ("NavigationExpenses", "/expenses", "bi bi-cash"),
                new ("NavigationCategories", "/expenses/categories", "bi bi-tags"),
                new ("NavigationImportTransactions", "/import", "bi bi-upload"),
                new ("NavigationTransactions", "/transactions", "bi bi-list-ul"),
                new ("Kategoryzuj transakcje", "/expenses/categorize", "bi bi-list-ul"),
                new ("Reguły kategoryzacji", "/expenses/configuration/category-rules", "bi bi-list-ul"),
                new ("Konfiguracja kategorii", "/expenses/configuration/categories", "bi bi-list-ul")
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