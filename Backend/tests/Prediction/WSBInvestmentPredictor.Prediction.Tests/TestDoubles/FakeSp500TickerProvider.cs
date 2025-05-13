using WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;

public class FakeSp500TickerProvider : ISp500TickerProvider
{
    private readonly IEnumerable<CompanyTicker> _tickers;

    public FakeSp500TickerProvider(IEnumerable<CompanyTicker> tickers)
    {
        _tickers = tickers;
    }

    public IEnumerable<CompanyTicker> GetAll() => _tickers;
}