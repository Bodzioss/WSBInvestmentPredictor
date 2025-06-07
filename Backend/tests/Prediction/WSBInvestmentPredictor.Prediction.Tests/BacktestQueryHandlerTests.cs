using WSBInvestmentPredictor.Prediction.Application.Queries;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using WSBInvestmentPredictor.Prediction.UnitTests.Builders;
using WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.UnitTests;

public class BacktestQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsBacktestPoints_WhenDataIsAvailable()
    {
        // Arrange
        var marketData = new RawMarketDataBuilder()
            .WithDateRange(new DateTime(2023, 10, 1), 150)
            .Build();

        var fakePolygonClient = new FakePolygonClient(marketData);
        var fakePredictionEngine = new FakePredictionEngine(0.05f); // stała predykcja +5%

        var handler = new BacktestQueryHandler(fakePolygonClient, fakePredictionEngine);
        var query = new RunBacktestQuery("TEST", 2024);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Points.Count > 0);
        Assert.All(result.Points, p => Assert.Equal(2024, p.Date.Year));
    }
}