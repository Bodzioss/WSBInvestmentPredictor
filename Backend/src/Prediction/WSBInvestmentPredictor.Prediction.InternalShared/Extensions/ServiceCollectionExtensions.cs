using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WSBInvestmentPredictor.Prediction.Application;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;
using WSBInvestmentPredictor.Prediction.Infrastructure;
using WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;
using WSBInvestmentPredictor.Prediction.Infrastructure.Prediction;
using WSBInvestmentPredictor.Prediction.MarketData;

namespace WSBInvestmentPredictor.Prediction.InternalShared.Extensions;

/// <summary>
/// Provides extension methods for configuring services in the Prediction module.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures all services required by the Prediction module.
    /// </summary>
    /// <param name="services">The IServiceCollection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured IServiceCollection instance.</returns>
    public static IServiceCollection AddPredictionModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register application (MediatR)
        services.AddPredictionApplication();

        // Register domain services
        services.AddScoped<IPredictionEngine, PredictionEngine>();
        services.AddScoped<IStockPredictorService, StockPredictorService>();
        services.AddScoped<IPolygonClient, PolygonClient>();
        services.AddScoped<ISp500TickerProvider, Sp500CsvTickerProvider>();
        services.AddScoped<MarketDataPredictionBuilder>();

        // Register HttpClient for PolygonClient
        services.AddHttpClient<IPolygonClient, PolygonClient>();

        // Add other Prediction module services here

        return services;
    }
}