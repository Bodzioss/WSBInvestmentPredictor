using WSBInvestmentPredictor.Backend.API.Cqrs;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Prediction.Shared.Queries;

namespace WSBInvestmentPredictor.Backend.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> ConfigureApplicationModules(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors("AllowFrontend");
        app.UseAuthorization();

        // Mapowanie endpointów z modułu Expenses
        app.MapCqrsEndpoints(typeof(AddTransactions).Assembly, "AllowFrontend");

        // Mapowanie endpointów z modułu Prediction
        app.MapCqrsEndpoints(typeof(GetApiStatusQuery).Assembly, "AllowFrontend");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
}