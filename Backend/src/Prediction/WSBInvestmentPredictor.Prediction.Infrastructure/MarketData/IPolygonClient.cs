using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.MarketData;

public interface IPolygonClient
{
    Task<List<RawMarketData>> GetDailyOhlcvAsync(string symbol, DateTime from, DateTime to);
}