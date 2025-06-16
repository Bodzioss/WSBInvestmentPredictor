using Microsoft.Extensions.DependencyInjection;
using WSBInvestmentPredictor.Prediction.Shared.Queries;

namespace WSBInvestmentPredictor.Prediction.Application;

public static class DI
{
    public static IServiceCollection AddPredictionApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DI).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(GetSp500TickersQuery).Assembly);
        });

        return services;
    }
}