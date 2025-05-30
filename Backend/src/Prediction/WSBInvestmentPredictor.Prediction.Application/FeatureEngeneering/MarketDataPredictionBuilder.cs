using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.Application.FeatureEngeneering;

public class MarketDataPredictionBuilder
{
    /// <summary>
    /// Przetwarza podstawowe dane OHLCV i zwraca listę gotowych rekordów MarketData z obliczonymi cechami.
    /// </summary>
    /// <param name="rawData">Lista podstawowych danych (z API lub CSV)</param>
    /// <returns>Lista MarketData gotowa do treningu/predykcji</returns>
    public List<MarketDataInput> Build(List<RawMarketData> rawData)
    {
        var results = new List<MarketDataInput>();
        for (int i = 20; i < rawData.Count - 30; i++) // zapewniamy okna SMA i target
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