using WSBInvestmentPredictor.Prediction.Application.Queries;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using WSBInvestmentPredictor.Prediction.UnitTests.Builders;
using WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.UnitTests;

public class GetSp500TickersHandlerTests
{
    [Fact]
    public async Task Returns_expected_tickers()
    {
        // Arrange
        var tickers = new List<CompanyTicker>
    {
        new CompanyTickerBuilder().WithTicker("AAPL").WithName("Apple Inc.").Build(),
        new CompanyTickerBuilder().WithTicker("MSFT").WithName("Microsoft").Build()
    };

        var mockData = new List<RawMarketData>
{
    new(DateTime.UtcNow.AddDays(-1).ToString(), 100, 110, 90, 105, 150_000),
    new(DateTime.UtcNow.ToString(), 106, 112, 101, 108, 120_000)
};

        var fakePolygon = new FakePolygonClient(mockData);
        var provider = new FakeSp500TickerProvider(tickers);
        var handler = new MarketDataQueryHandler(provider, fakePolygon);

        // Act
        var result = await handler.Handle(new GetSp500TickersQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("AAPL", result[0].Ticker);
        Assert.Equal("Microsoft", result[1].Name);
    }

    [Fact]
    public async Task Returns_empty_list_when_no_data()
    {
        var mockData = new List<RawMarketData>
{
    new(DateTime.UtcNow.AddDays(-1).ToString(), 100, 110, 90, 105, 150_000),
    new(DateTime.UtcNow.ToString(), 106, 112, 101, 108, 120_000)
};

        var fakePolygon = new FakePolygonClient(mockData);
        // Arrange
        var provider = new FakeSp500TickerProvider([]);
        var handler = new MarketDataQueryHandler(provider, fakePolygon);

        // Act
        var result = await handler.Handle(new GetSp500TickersQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}