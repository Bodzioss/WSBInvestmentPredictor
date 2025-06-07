using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.Infrastructure.Prediction;

public class PredictionEngine : IPredictionEngine
{
    public Task<PredictionResult> PredictAsync(List<RawMarketData> history)
    {
        // Wersja testowa (stała wartość):
        return Task.FromResult(new PredictionResult
        {
            Score = 0.5f,
            ChangePercentage = 0.05f
        });
    }
}