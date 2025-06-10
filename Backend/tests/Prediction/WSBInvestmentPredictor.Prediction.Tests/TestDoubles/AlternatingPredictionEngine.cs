using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;

/// <summary>
/// Test double: naprzemiennie zwraca dodatnie i ujemne predykcje.
/// Umożliwia testowanie częściowo trafnych predykcji.
/// </summary>
public class AlternatingPredictionEngine : IPredictionEngine
{
    private bool _flip = false;

    public Task<PredictionResult> PredictAsync(List<RawMarketData> history)
    {
        _flip = !_flip;
        float prediction = _flip ? 0.1f : -0.1f;
        return Task.FromResult(new PredictionResult { ChangePercentage = prediction });
    }
}