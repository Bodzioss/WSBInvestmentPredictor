using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.Infrastructure.Prediction;

public class PredictionEngine : IPredictionEngine
{
    private readonly StockPredictorService _predictorService;
    private readonly MarketDataPredictionBuilder _featureBuilder;

    public PredictionEngine()
    {
        _predictorService = new StockPredictorService();
        _featureBuilder = new MarketDataPredictionBuilder();
    }

    public Task<PredictionResult> PredictAsync(List<RawMarketData> history)
    {
        // Budujemy cechy z raw danych
        var featureData = _featureBuilder.Build(history);

        if (featureData.Count == 0)
        {
            return Task.FromResult(new PredictionResult
            {
                Score = 0,
                ChangePercentage = 0
            });
        }

        // Trenujemy model na dostępnych danych (lub w praktyce z zewnętrznego źródła)
        _predictorService.Train(featureData);

        // Bierzemy ostatni element jako bazę predykcji
        var latestSample = featureData.Last();

        // Predykcja zwraca "Score" - czyli prognozowaną zmianę
        var predictionScore = _predictorService.Predict(latestSample);

        return Task.FromResult(new PredictionResult
        {
            Score = predictionScore,
            ChangePercentage = predictionScore // możesz mapować inaczej jeśli chcesz
        });
    }
}