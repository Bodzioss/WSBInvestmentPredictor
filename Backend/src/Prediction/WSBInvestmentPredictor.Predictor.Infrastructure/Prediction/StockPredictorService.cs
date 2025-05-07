using Microsoft.ML;
using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;

namespace WSBInvestmentPredictor.Predictor.Infrastructure.Prediction;

/// <summary>
/// Service responsible for training a regression model and making predictions based on market data.
/// </summary>
public class StockPredictorService : IStockPredictorService, IModelTrainer
{
    private readonly MLContext _mlContext;
    private ITransformer? _model;

    public StockPredictorService()
    {
        _mlContext = new MLContext();
    }

    /// <summary>
    /// Trains a FastTree regression model using the provided market data.
    /// </summary>
    /// <param name="data">A collection of market data records used for training.</param>
    public void Train(IEnumerable<MarketDataInput> data)
    {
        var dataView = _mlContext.Data.LoadFromEnumerable(data);

        var pipeline = _mlContext.Transforms.CopyColumns("Label", nameof(MarketDataInput.Target))
            .Append(_mlContext.Transforms.Concatenate("Features",
                nameof(MarketDataInput.Open),
                nameof(MarketDataInput.High),
                nameof(MarketDataInput.Low),
                nameof(MarketDataInput.Close),
                nameof(MarketDataInput.Volume),
                nameof(MarketDataInput.SMA_5),
                nameof(MarketDataInput.SMA_10),
                nameof(MarketDataInput.SMA_20),
                nameof(MarketDataInput.Volatility_10),
                nameof(MarketDataInput.RSI_14)))
            .Append(_mlContext.Regression.Trainers.FastTree());

        _model = pipeline.Fit(dataView);
    }

    /// <summary>
    /// Predicts the 30-day return using a trained model and a new market data sample.
    /// </summary>
    /// <param name="input">A single market data sample for which to make a prediction.</param>
    /// <returns>Predicted return as a float score.</returns>
    public float Predict(MarketDataInput input)
    {
        if (_model == null)
            throw new InvalidOperationException("Model has not been trained. Call Train() first.");

        var engine = _mlContext.Model.CreatePredictionEngine<MarketDataInput, PredictionResult>(_model);
        var result = engine.Predict(input);

        return result.Score;
    }
}