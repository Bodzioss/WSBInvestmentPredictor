using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.Domain.Interfaces;

public interface IPredictionEngine
{
    Task<PredictionResult> PredictAsync(List<RawMarketData> history);
}