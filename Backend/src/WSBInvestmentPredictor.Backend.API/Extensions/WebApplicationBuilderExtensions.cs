using WSBInvestmentPredictor.Expenses.InternalShared.Extensions;
using WSBInvestmentPredictor.Prediction.InternalShared.Extensions;

namespace WSBInvestmentPredictor.Backend.API.Extensions;

/// <summary>
/// Provides extension methods for configuring the WebApplicationBuilder and its services.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the basic application services including controllers, API explorer, Swagger, and CORS.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance to configure.</param>
    /// <returns>The configured WebApplicationBuilder instance.</returns>
    public static WebApplicationBuilder ConfigureApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(
                        "https://localhost:7236",
                        "http://localhost:7236",
                        "https://localhost:5175",
                        "http://localhost:5175",
                        "https://wsbinvestmentpredictor-frontend-g6gegxf5gdhnbpe8.polandcentral-01.azurewebsites.net",
                        "https://wsbinvestmentpredictor-frontend-dev-fzgbh8hcdbafbhaw.polandcentral-01.azurewebsites.net",
                        "https://wsbinvestmentpredictor.azurewebsites.net",
                        "https://wsbinvestmentpredictor-dev.polandcentral-01.azurewebsites.net"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return builder;
    }

    /// <summary>
    /// Configures services for all application modules (Expenses and Prediction).
    /// </summary>
    /// <param name="services">The IServiceCollection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured IServiceCollection instance.</returns>
    public static IServiceCollection ConfigureModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Expenses module
        services.AddExpensesModule(configuration);
        
        // Configure Prediction module
        services.AddPredictionModule(configuration);

        return services;
    }
}