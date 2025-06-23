using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WSBInvestmentPredictor.Expenses.Application;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Domain.Services;
using WSBInvestmentPredictor.Expenses.Infrastructure.Repositories;

namespace WSBInvestmentPredictor.Expenses.InternalShared.Extensions;

/// <summary>
/// Provides extension methods for configuring services in the Expenses module.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures all services required by the Expenses module.
    /// </summary>
    /// <param name="services">The IServiceCollection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured IServiceCollection instance.</returns>
    public static IServiceCollection AddExpensesModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Rejestracja aplikacji (MediatR)
        services.AddExpensesApplication();

        // Rejestracja serwisów domenowych
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}