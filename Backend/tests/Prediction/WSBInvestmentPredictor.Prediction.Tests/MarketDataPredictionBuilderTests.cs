using System.Globalization;
using WSBInvestmentPredictor.Prediction.Application.FeatureEngeneering;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.UnitTests;

public class MarketDataPredictionBuilderTests
{
    private readonly MarketDataPredictionBuilder _builder = new();

    private List<RawMarketData> GenerateRawData(int count, Func<int, float> closeSelector = null)
    {
        closeSelector ??= i => 100 + i;

        return Enumerable.Range(0, count).Select(i =>
            new RawMarketData(
                Date: DateTime.UtcNow.Date.AddDays(-count + i).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Open: 100,
                High: 110,
                Low: 90,
                Close: closeSelector(i),
                Volume: 1000)
        ).ToList();
    }

    [Fact]
    public void Build_ReturnsEmpty_WhenNotEnoughData()
    {
        var input = GenerateRawData(40);
        var result = _builder.Build(input);
        Assert.Empty(result);
    }

    [Fact]
    public void Build_ReturnsCorrectNumberOfRecords()
    {
        var input = GenerateRawData(100);
        var result = _builder.Build(input);
        Assert.Equal(50, result.Count); // 100 - 30 - 20 = 50
    }

    [Fact]
    public void Build_ComputesCorrectSMA5()
    {
        var input = GenerateRawData(100);
        var result = _builder.Build(input);

        var sample = result.First();
        float expected = (float)Enumerable.Range(15, 5).Average(i => 100 + i);
        Assert.Equal(expected, sample.SMA_5, 2);
    }

    [Fact]
    public void Build_ComputesPositiveTarget_WhenPricesGrow()
    {
        var input = GenerateRawData(100);
        var result = _builder.Build(input);

        Assert.All(result, r => Assert.True(r.Target > 0));
    }

    [Fact]
    public void Build_HandlesFlatPrices()
    {
        var input = GenerateRawData(100, _ => 100f);
        var result = _builder.Build(input);

        Assert.All(result, r =>
        {
            Assert.Equal(100f, r.Close);
            Assert.Equal(100f, r.SMA_5);
            Assert.Equal(100f, r.SMA_10);
            Assert.Equal(100f, r.SMA_20);
            Assert.Equal(0f, r.Volatility_10);
            Assert.Equal(0f, r.Target);
            Assert.Equal(100f, r.RSI_14);
        });
    }
}