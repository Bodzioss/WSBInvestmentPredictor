using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.MarketData;

public interface IPolygonClient
{
    Task<List<RawMarketData>> GetDailyOhlcvAsync(string symbol, DateTime from, DateTime to);
}