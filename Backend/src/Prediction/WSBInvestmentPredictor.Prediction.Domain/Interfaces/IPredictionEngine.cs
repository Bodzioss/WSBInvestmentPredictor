using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.Domain.Interfaces;

public interface IPredictionEngine
{
    Task<PredictionResult> PredictAsync(List<RawMarketData> history);
}