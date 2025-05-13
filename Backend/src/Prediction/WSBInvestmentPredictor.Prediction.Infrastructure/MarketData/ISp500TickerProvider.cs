using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;

public interface ISp500TickerProvider
{
    IEnumerable<CompanyTicker> GetAll();
}