using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.Domain.Interfaces;

public interface IStockPredictorService
{
    void Train(IEnumerable<MarketDataInput> data);

    float Predict(MarketDataInput input);
}