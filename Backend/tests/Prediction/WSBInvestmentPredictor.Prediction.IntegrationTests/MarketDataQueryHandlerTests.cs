using System.Net.Http.Json;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.IntegrationTests;

public class MarketDataQueryHandlerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MarketDataQueryHandlerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSp500Tickers_Endpoint_ReturnsSuccessAndList()
    {
        var resourcesPath = Path.Combine(AppContext.BaseDirectory, "Resources", "sp500.csv");
        Assert.True(File.Exists(resourcesPath), $"Nie znaleziono pliku: {resourcesPath}");

        var response = await _client.GetAsync("/api/MarketData/tickers");
        var content = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"Status: {(int)response.StatusCode}\nContent: {content}");

        var tickers = await response.Content.ReadFromJsonAsync<List<CompanyTicker>>();
        Assert.NotNull(tickers);
        Assert.NotEmpty(tickers);
    }
}