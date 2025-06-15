
using WSBInvestmentPredictor.Expenses.InternalShared.Extensions;
using WSBInvestmentPredictor.Prediction.InternalShared.Extensions;

namespace WSBInvestmentPredictor.Backend.API.Extensions;

public static class WebApplicationBuilderExtensions
{
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
                        "https://wsbinvestmentpredictor-frontend-g6gegxf5gdhnbpe8.polandcentral-01.azurewebsites.net",
                        "https://wsbinvestmentpredictor.azurewebsites.net"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return builder;
    }

    public static IServiceCollection ConfigureModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Konfiguracja modułu Expenses
        services.AddExpensesModule(configuration);

        // Konfiguracja modułu Prediction
        services.AddPredictionModule(configuration);

        return services;
    }
} 