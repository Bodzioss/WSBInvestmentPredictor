using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WSBInvestmentPredictor.Prediction.Application;

public static class DI
{
    public static IServiceCollection AddPredictionApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DI).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        });

        return services;
    }
}
