using Microsoft.EntityFrameworkCore;
using WSBInvestmentPredictor.Backend.API.Cqrs;
using WSBInvestmentPredictor.Expenses.Infrastructure.Data;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using WSBInvestmentPredictor.Technology.Middleware;

namespace WSBInvestmentPredictor.Backend.API.Extensions;

/// <summary>
/// Provides extension methods for configuring the WebApplication and its middleware pipeline.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the application's middleware pipeline and maps endpoints for all modules.
    /// </summary>
    /// <param name="app">The WebApplication instance to configure.</param>
    /// <returns>The configured WebApplication instance.</returns>
    public static async Task<WebApplication> ConfigureApplicationModules(this WebApplication app)
    {
        // Ensure database is created and migrations are applied
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ExpensesDbContext>();
            await context.Database.MigrateAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors("AllowFrontend");
        app.UseHttpsRedirection();
        app.UseAuthorization();

        // Add error handling middleware before endpoint mapping
        app.UseErrorHandling();

        // Map endpoints for all modules
        app.MapControllers();
        app.MapCqrsEndpoints(typeof(AddTransactions).Assembly, "AllowFrontend");
        app.MapCqrsEndpoints(typeof(GetApiStatusQuery).Assembly, "AllowFrontend");

        return app;
    }
}