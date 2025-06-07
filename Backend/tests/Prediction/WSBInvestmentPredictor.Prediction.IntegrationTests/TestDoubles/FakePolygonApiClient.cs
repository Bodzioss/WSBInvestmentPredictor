using WSBInvestmentPredictor.Prediction.MarketData;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.IntegrationTests.TestDoubles;

public class FakePolygonClient : IPolygonClient
{
    public Task<List<RawMarketData>> GetDailyOhlcvAsync(string symbol, DateTime from, DateTime to)
    {
        var days = (to - from).Days + 1;

        var data = Enumerable.Range(0, days).Select(i =>
            new RawMarketData(
                from.AddDays(i).ToString("yyyy-MM-dd"),
                Open: 100 + i,
                High: 110 + i,
                Low: 90 + i,
                Close: 100 + i,
                Volume: 100000
            )).ToList();

        return Task.FromResult(data);
    }
}