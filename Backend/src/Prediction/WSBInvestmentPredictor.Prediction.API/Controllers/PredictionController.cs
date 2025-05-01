using Microsoft.AspNetCore.Mvc;
using WSBInvestmentPredictor.Prediction.Domain.Entities;
using WSBInvestmentPredictor.Prediction.Services.FeatureEngeneering;
using WSBInvestmentPredictor.Prediction.Services.Prediction;

namespace WSBInvestmentPredictor.Prediction.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly StockPredictorService _predictor;
    private readonly MarketDataBuilder _builder;

    public PredictionController(StockPredictorService predictor, MarketDataBuilder builder)
    {
        _predictor = predictor;
        _builder = builder;
    }

    /// <summary>
    /// Predicts the 30-day return based on the provided market data.
    /// </summary>
    /// <param name="data">A market data sample (most recent day).</param>
    /// <returns>Predicted return score.</returns>
    [HttpPost]
    public IActionResult Predict([FromBody] MarketData data)
    {
        try
        {
            var result = _predictor.Predict(data);
            return Ok(new { prediction = result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Trains the model using a list of market data.
    /// </summary>
    /// <param name="trainingData">List of historical market data.</param>
    [HttpPost("train")]
    public IActionResult Train([FromBody] List<MarketData> trainingData)
    {
        try
        {
            _predictor.Train(trainingData);
            return Ok("Model trained successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Trains the model and returns the prediction based on raw OHLCV data.
    /// </summary>
    /// <param name="rawData">List of basic market data (e.g. from external source).</param>
    /// <returns>Prediction score for the last available sample.</returns>
    [HttpPost("predict-from-raw")]
    public IActionResult PredictFromRaw([FromBody] List<RawMarketData> rawData)
    {
        try
        {
            var processed = _builder.Build(rawData);
            if (processed.Count == 0)
                return BadRequest("Insufficient data for feature engineering.");

            _predictor.Train(processed); // lub opcjonalnie tylko raz
            var prediction = _predictor.Predict(processed.Last());

            return Ok(new { prediction });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}