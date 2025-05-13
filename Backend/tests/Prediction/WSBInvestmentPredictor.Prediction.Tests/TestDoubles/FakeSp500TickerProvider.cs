using System.Collections.Generic;
using WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;
using WSBInvestmentPredictor.Prediction.InternalShared.ValueObjects;

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