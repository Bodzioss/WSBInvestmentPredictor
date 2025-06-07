using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.IntegrationTests;

public class MarketDataQueryHandlerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MarketDataQueryHandlerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSp500Tickers_Endpoint_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync("/api/MarketData/tickers");

        var content = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"Status: {(int)response.StatusCode}\nContent: {content}");

        Assert.False(string.IsNullOrWhiteSpace(content));
    }
}
