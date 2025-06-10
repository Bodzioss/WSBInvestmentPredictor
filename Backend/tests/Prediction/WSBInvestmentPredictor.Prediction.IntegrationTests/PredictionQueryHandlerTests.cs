using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using WSBInvestmentPredictor.Prediction.Application.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.IntegrationTests;

public class PredictionQueryHandlerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PredictionQueryHandlerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PredictFromRaw_WithEnoughData_ReturnsPrediction()
    {
        var rawData = Enumerable.Range(1, 100).Select(i =>
            new RawMarketData(
                DateTime.Parse("2023-01-01").AddDays(i).ToString("yyyy-MM-dd"),
                Open: 100 + i,
                High: 110 + i,
                Low: 90 + i,
                Close: 100 + i,
                Volume: 100_000
            )).ToList();

        var request = new PredictFromRawQuery(rawData);

        var response = await _client.PostAsJsonAsync("/api/Prediction/predict-from-raw", request);

        var content = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"Unexpected status {response.StatusCode}: {content}");

        var result = await response.Content.ReadFromJsonAsync<PredictionResultDto>();
        Assert.NotNull(result);
        Assert.InRange(result.Prediction, -1f, 1f);
    }

    [Fact]
    public async Task PredictFromRaw_WithInsufficientData_ReturnsBadRequest()
    {
        var rawData = new List<RawMarketData>
        {
            new("2023-01-01", 100, 110, 90, 100, 100000),
            new("2023-01-02", 101, 111, 91, 101, 100000),
            new("2023-01-03", 102, 112, 92, 102, 100000)
        };

        var request = new PredictFromRawQuery(rawData);

        var response = await _client.PostAsJsonAsync("/api/Prediction/predict-from-raw", request);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Insufficient data for feature engineering", content);
    }
}