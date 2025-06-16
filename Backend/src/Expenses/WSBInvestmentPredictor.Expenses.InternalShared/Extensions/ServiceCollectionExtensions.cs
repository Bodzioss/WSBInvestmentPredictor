using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WSBInvestmentPredictor.Expenses.Application;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Domain.Services;
using WSBInvestmentPredictor.Expenses.Infrastructure.Repositories;

namespace WSBInvestmentPredictor.Expenses.InternalShared.Extensions;

public static class ServiceCollectionExtensions
{
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