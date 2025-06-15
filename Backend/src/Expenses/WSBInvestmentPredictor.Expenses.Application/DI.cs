using Microsoft.Extensions.DependencyInjection;

namespace WSBInvestmentPredictor.Expenses.Application;

public static class DI
{
    public static IServiceCollection AddExpensesApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DI).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        });

        return services;
    }
}