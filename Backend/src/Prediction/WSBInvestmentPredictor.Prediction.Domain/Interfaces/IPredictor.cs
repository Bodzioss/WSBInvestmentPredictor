namespace WSBInvestmentPredictor.Prediction.Domain.Interfaces;

/// <summary>
/// Provides an interface for predicting future values based on market data input.
/// </summary>
public interface IPredictor
{
    /// <summary>
    /// Predicts the future return (e.g., 30-day return) for a given market data input.
    /// </summary>
    /// <param name="input">Market data sample to predict.</param>
    /// <returns>Predicted return as a float (regression score).</returns>
    float Predict(Entities.MarketData input);
}
