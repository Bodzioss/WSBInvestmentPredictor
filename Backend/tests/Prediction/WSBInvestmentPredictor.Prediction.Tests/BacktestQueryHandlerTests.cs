using WSBInvestmentPredictor.Prediction.Application.Queries;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
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
        var fakePredictionEngine = new FakePredictionEngine(0.05f); // constant prediction +5%

        var handler = new BacktestQueryHandler(fakePolygonClient, fakePredictionEngine);
        var query = new RunBacktestQuery("TEST", 2024);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Points.Count > 0);
        Assert.All(result.Points, p => Assert.Equal(2024, p.Date.Year));
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenNoMarketData()
    {
        // Arrange
        var fakePolygonClient = new FakePolygonClient(new List<RawMarketData>());
        var fakePredictionEngine = new FakePredictionEngine(0.0f);

        var handler = new BacktestQueryHandler(fakePolygonClient, fakePredictionEngine);
        var query = new RunBacktestQuery("TEST", 2023);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Points);
        Assert.Equal(0f, result.Accuracy);
        Assert.Equal(0f, result.MeanSquaredError);
    }

    [Fact]
    public async Task Handle_SkipsPoints_WhenNotEnoughHistoricalData()
    {
        // Arrange - only 10 days of data = too little for prediction
        var marketData = new RawMarketDataBuilder()
            .WithDateRange(new DateTime(2024, 1, 1), 10) 
            .Build();


        var fakePolygonClient = new FakePolygonClient(marketData);
        var fakePredictionEngine = new FakePredictionEngine(0.1f);

        var handler = new BacktestQueryHandler(fakePolygonClient, fakePredictionEngine);
        var query = new RunBacktestQuery("TEST", 2024);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Points);
    }

    [Fact]
    public async Task Handle_SkipsPoints_WhenNoFutureCandleAvailable()
    {
        // Arrange – no candles for 30 days ahead
        // Arrange – brak świec za 30 dni
        var marketData = new RawMarketDataBuilder()
            .WithDateRange(new DateTime(2024, 1, 1), 60) 
            .Build();

        var fakePolygonClient = new FakePolygonClient(marketData);
        var fakePredictionEngine = new FakePredictionEngine(0.1f);

        var handler = new BacktestQueryHandler(fakePolygonClient, fakePredictionEngine);
        var query = new RunBacktestQuery("TEST", 2024);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Points);
    }

    [Fact]
    public async Task Handle_CalculatesCorrectAccuracyAndMSE()
    {
        // Arrange
        var builder = new RawMarketDataBuilder();
        var marketData = builder
            .WithDateRange(new DateTime(2023, 1, 1), 400, i => 100 * (float)Math.Pow(1.001, i))
            .Build();

        var fakePolygonClient = new FakePolygonClient(marketData);
        var fakePredictionEngine = new FakePredictionEngine(0.03f);

        var handler = new BacktestQueryHandler(fakePolygonClient, fakePredictionEngine);
        var query = new RunBacktestQuery("TEST", 2023);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Points.Count > 0);
        Assert.Equal(1f, result.Accuracy); // all signs correct
        Assert.True(result.MeanSquaredError < 0.0001f);
    }

    [Fact]
    public async Task Handle_ReturnsZeroAccuracyAndMSE_WhenNoPointsGenerated()
    {
        // Arrange - only 10 days of data = too little for prediction
        var marketData = new RawMarketDataBuilder()
            .WithDateRange(new DateTime(2023, 1, 1), 10)
            .Build();

        var fakePolygonClient = new FakePolygonClient(marketData);
        var fakePredictionEngine = new FakePredictionEngine(0.1f);

        var handler = new BacktestQueryHandler(fakePolygonClient, fakePredictionEngine);
        var query = new RunBacktestQuery("TEST", 2023);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Points);
        Assert.Equal(0f, result.Accuracy);
        Assert.Equal(0f, result.MeanSquaredError);
    }

    [Fact]
    public async Task Handle_ReturnsZeroAccuracy_WhenPredictionsAlwaysWrongDirection()
    {
        // Arrange - prices are rising (positive actual change)
        var marketData = new RawMarketDataBuilder()
            .WithDateRange(new DateTime(2023, 1, 1), 400, i => 100 + i) // wzrosty
            .Build();


        var fakePolygonClient = new FakePolygonClient(marketData);
        var fakePredictionEngine = new FakePredictionEngine(-0.01f); // prediction says: falling (negative sign)

        var handler = new BacktestQueryHandler(fakePolygonClient, fakePredictionEngine);
        var query = new RunBacktestQuery("TEST", 2023);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Points.Count > 0);
        Assert.Equal(0f, result.Accuracy); // wszystko nietrafione
        Assert.True(result.MeanSquaredError > 0.01f);
    }

    [Fact]
    public async Task Handle_ReturnsHalfAccuracy_WhenPredictionsAlternate()
    {
        var marketData = new RawMarketDataBuilder()
            .WithDateRange(new DateTime(2023, 1, 1), 400, i => 100 + i)
            .Build();

        var fakePolygonClient = new FakePolygonClient(marketData);
        var alternatingEngine = new AlternatingPredictionEngine();

        var handler = new BacktestQueryHandler(fakePolygonClient, alternatingEngine);
        var query = new RunBacktestQuery("TEST", 2023);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.Points.Count > 0);
        Assert.InRange(result.Accuracy, 0.45f, 0.55f); // pozwalamy na lekki margines
    }
}