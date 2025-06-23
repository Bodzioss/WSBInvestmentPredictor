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
        // Rejestracja aplikacji (MediatR)
        services.AddPredictionApplication();

        // Rejestracja serwisów domenowych
        services.AddScoped<IStockPredictorService, StockPredictorService>();
        services.AddSingleton<ISp500TickerProvider, Sp500CsvTickerProvider>();
        services.AddSingleton<MarketDataPredictionBuilder>();
        services.AddHttpClient<IPolygonClient, PolygonClient>();
        services.AddScoped<IPredictionEngine, PredictionEngine>();
        // Tutaj dodaj inne serwisy z modułu Prediction

        return services;
    }
}