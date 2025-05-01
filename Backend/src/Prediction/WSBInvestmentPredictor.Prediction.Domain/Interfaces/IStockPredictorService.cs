using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.Domain.Interfaces;

public interface IStockPredictorService
{
    void Train(IEnumerable<MarketData> data);
    float Predict(MarketData input);
}