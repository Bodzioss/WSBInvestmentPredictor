using Microsoft.Extensions.DependencyInjection;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

namespace WSBInvestmentPredictor.Expenses.Application;

public static class DI
{
    public static IServiceCollection AddExpensesApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DI).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(AddTransactions).Assembly);
        });

        return services;
    }
}