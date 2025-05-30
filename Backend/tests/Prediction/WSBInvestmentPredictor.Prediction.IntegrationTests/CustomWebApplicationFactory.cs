using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WSBInvestmentPredictor.Prediction.IntegrationTests.TestDoubles;
using WSBInvestmentPredictor.Prediction.MarketData;

namespace WSBInvestmentPredictor.Prediction.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPolygonClient>();
            services.AddSingleton<IPolygonClient, FakePolygonClient>();
        });
    }
}