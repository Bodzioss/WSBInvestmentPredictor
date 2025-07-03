using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.Infrastructure;

public class MarketDataPredictionBuilder
{
    /// <summary>
    /// Processes basic OHLCV data and returns a list of ready MarketData records with calculated features.
    /// </summary>
    /// <param name="rawData">List of basic data (from API or CSV)</param>
    /// <returns>List of MarketData ready for training/prediction</returns>
    public List<MarketDataInput> Build(List<RawMarketData> rawData)
    {
        if (rawData.Count < 50) return new List<MarketDataInput>();

        var results = new List<MarketDataInput>();

        // Minimum index from which all indicators can be calculated (20-day window and 14-day RSI)
        int startIndex = 20;
        
        // Maximum index to have 30 days for target forward
        int endIndex = rawData.Count - 31; // -31 because i+30 must be < Count

        for (int i = startIndex; i <= endIndex; i++)
        {
            var window5 = rawData.Skip(i - 5).Take(5).Select(x => x.Close);
            var window10 = rawData.Skip(i - 10).Take(10).Select(x => x.Close);
            var window20 = rawData.Skip(i - 20).Take(20).Select(x => x.Close);

            var sma5 = window5.Average();
            var sma10 = window10.Average();
            var sma20 = window20.Average();

            var returns = rawData.Skip(i - 10).Take(10)
                .Select((x, idx) => idx == 0 ? 0 : Math.Log(x.Close / rawData[i - 10 + idx - 1].Close));
            var volatility = StdDev(returns);

            var rsi = CalculateRSI(rawData.Skip(i - 14).Take(14).ToList());

            var target = (rawData[i + 30].Close - rawData[i].Close) / rawData[i].Close;

            var current = rawData[i];
            results.Add(new MarketDataInput(
                current.Date,
                current.Open,
                current.High,
                current.Low,
                current.Close,
                current.Volume,
                (float)sma5,
                (float)sma10,
                (float)sma20,
                (float)volatility,
                (float)rsi,
                (float)target
            ));
        }

        return results;
    }

    private static double StdDev(IEnumerable<double> values)
    {
        var arr = values.ToArray();
        var avg = arr.Average();
        return Math.Sqrt(arr.Sum(v => Math.Pow(v - avg, 2)) / arr.Length);
    }

    private static double CalculateRSI(List<RawMarketData> data)
    {
        double gain = 0, loss = 0;
        for (int i = 1; i < data.Count; i++)
        {
            var delta = data[i].Close - data[i - 1].Close;
            if (delta > 0) gain += delta;
            else loss -= delta; // loss jest dodatni
        }
        if (loss == 0) return 100;
        var rs = gain / loss;
        return 100 - 100 / (1 + rs);
    }
}