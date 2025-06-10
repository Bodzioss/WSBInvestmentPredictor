using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.IntegrationTests;

public class CqrsEndpointMapperTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CqrsEndpointMapperTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/api/Prediction/predict-from-raw", "POST")]
    [InlineData("/api/backtest", "POST")]
    [InlineData("/api/MarketData/tickers", "GET")]
    public void Should_register_expected_cqrs_endpoint(string expectedPath, string expectedMethod)
    {
        // Arrange
        var server = _factory.Server;
        var dataSource = server.Services.GetRequiredService<EndpointDataSource>();

        var endpoints = dataSource.Endpoints
            .OfType<RouteEndpoint>()
            .Select(e => new
            {
                Path = e.RoutePattern.RawText,
                Methods = e.Metadata
                    .OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
                    .FirstOrDefault()?.HttpMethods
            })
            .ToList();

        // Act
        var match = endpoints.Any(e =>
            e.Path.Equals(expectedPath, StringComparison.OrdinalIgnoreCase) &&
            e.Methods != null &&
            e.Methods.Contains(expectedMethod, StringComparer.OrdinalIgnoreCase));

        // Assert
        Assert.True(match, $"Endpoint {expectedMethod} {expectedPath} was not registered.");
    }
}