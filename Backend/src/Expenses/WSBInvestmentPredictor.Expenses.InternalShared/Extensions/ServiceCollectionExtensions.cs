using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Domain.Services;
using WSBInvestmentPredictor.Expenses.Infrastructure.Repositories;

namespace WSBInvestmentPredictor.Expenses.InternalShared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExpensesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Tutaj dodaj inne serwisy z modułu Expenses

        return services;
    }
}