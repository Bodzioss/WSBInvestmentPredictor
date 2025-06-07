using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;

public class FakePredictionEngine : IPredictionEngine
{
    private readonly float _fixedChange;

    public FakePredictionEngine(float fixedChange)
    {
        _fixedChange = fixedChange;
    }

    public Task<PredictionResult> PredictAsync(List<RawMarketData> history)
    {
        return Task.FromResult(new PredictionResult
        {
            Score = _fixedChange,
            ChangePercentage = _fixedChange
        });
    }
}