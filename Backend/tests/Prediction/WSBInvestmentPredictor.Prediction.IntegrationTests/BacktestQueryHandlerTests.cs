using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using WSBInvestmentPredictor.Prediction.Application.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.IntegrationTests;

public class BacktestQueryHandlerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BacktestQueryHandlerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Backtest_Endpoint_ReturnsResult()
    {
        var query = new RunBacktestQuery("AAPL", 2023);
        var response = await _client.PostAsJsonAsync("/api/backtest", query);

        var content = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"Status: {(int)response.StatusCode}\nContent: {content}");

        var result = await response.Content.ReadFromJsonAsync<BacktestResultDto>();
        Assert.NotNull(result);
        Assert.True(result.Points.Count >= 0);
    }
}
