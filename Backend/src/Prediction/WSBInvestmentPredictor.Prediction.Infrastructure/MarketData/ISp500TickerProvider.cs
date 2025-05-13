using WSBInvestmentPredictor.Prediction.InternalShared.ValueObjects;

namespace WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;

public interface ISp500TickerProvider
{
    IEnumerable<CompanyTicker> GetAll();
}
