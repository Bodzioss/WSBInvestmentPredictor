using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.UnitTests.Builders;

/// <summary>
/// Builder for creating valid MarketData objects in tests.
/// </summary>
public class MarketDataBuilder
{
    private string _date = "2025-04-01";
    private float _open = 100;
    private float _high = 105;
    private float _low = 98;
    private float _close = 104;
    private float _volume = 1_000_000;
    private float _sma5 = 102;
    private float _sma10 = 101;
    private float _sma20 = 99;
    private float _volatility10 = 0.015f;
    private float _rsi14 = 55;
    private float _target = 0.03f;

    public MarketDataBuilder WithTarget(float target)
    {
        _target = target;
        return this;
    }

    public MarketDataBuilder WithDate(string date)
    {
        _date = date;
        return this;
    }

    public MarketDataInput Build()
    {
        return new MarketDataInput(
            _date,
            _open,
            _high,
            _low,
            _close,
            _volume,
            _sma5,
            _sma10,
            _sma20,
            _volatility10,
            _rsi14,
            _target);
    }

    public List<MarketDataInput> BuildMany(int count)
    {
        return Enumerable.Range(1, count).Select(i =>
            WithDate($"2025-04-{i:D2}").WithTarget(0.01f * i).Build()
        ).ToList();
    }
}