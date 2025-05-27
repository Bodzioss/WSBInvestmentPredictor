using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.UnitTests.Builders;

public class RawMarketDataBuilder
{
    private readonly List<RawMarketData> _data = new();

    public RawMarketDataBuilder WithDateRange(DateTime start, int days, float basePrice = 100)
    {
        for (int i = 0; i < days; i++)
        {
            var date = start.AddDays(i).ToString("yyyy-MM-dd");
            var price = basePrice + i;

            _data.Add(new RawMarketData(
                Date: date,
                Open: price - 1,
                High: price + 2,
                Low: price - 2,
                Close: price,
                Volume: 1_000_000f
            ));
        }

        return this;
    }

    public List<RawMarketData> Build() => _data;
}