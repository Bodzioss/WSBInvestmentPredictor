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
        // Build features from raw data
        var featureData = _featureBuilder.Build(history);

        if (featureData.Count == 0)
        {
            return Task.FromResult(new PredictionResult
            {
                Score = 0,
                ChangePercentage = 0
            });
        }

        // Train model on available data (or in practice from external source)
        _predictorService.Train(featureData);

        // Take the last element as prediction base
        var latestSample = featureData.Last();

        // Prediction returns "Score" - i.e., predicted change
        var predictionScore = _predictorService.Predict(latestSample);

        return Task.FromResult(new PredictionResult
        {
            Score = predictionScore,
            ChangePercentage = predictionScore // you can map differently if needed
        });
    }
}