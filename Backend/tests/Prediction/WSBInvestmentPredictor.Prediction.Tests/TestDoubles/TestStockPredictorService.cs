using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;

namespace WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;

/// <summary>
/// Simple fake implementation of IStockPredictorService for unit testing.
/// </summary>
public class TestStockPredictorService : IStockPredictorService
{
    public IEnumerable<MarketData>? TrainedData { get; private set; }
    public bool WasTrained => TrainedData != null;
    public float PredictionToReturn { get; set; } = 0.0f;

    public void Train(IEnumerable<MarketData> data)
    {
        TrainedData = data;
    }

    public float Predict(MarketData input)
    {
        return PredictionToReturn;
    }
}