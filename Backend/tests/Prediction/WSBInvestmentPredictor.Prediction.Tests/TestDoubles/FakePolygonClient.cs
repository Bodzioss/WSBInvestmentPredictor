using WSBInvestmentPredictor.Prediction.MarketData;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;

public class FakePolygonClient : IPolygonClient
{
    private readonly List<RawMarketData> _data;

    public FakePolygonClient(List<RawMarketData> data)
    {
        _data = data;
    }

    public Task<List<RawMarketData>> GetDailyOhlcvAsync(string symbol, DateTime from, DateTime to)
    {
        var result = _data
            .Where(d => DateTime.Parse(d.Date) >= from && DateTime.Parse(d.Date) <= to)
            .ToList();

        return Task.FromResult(result);
    }
}