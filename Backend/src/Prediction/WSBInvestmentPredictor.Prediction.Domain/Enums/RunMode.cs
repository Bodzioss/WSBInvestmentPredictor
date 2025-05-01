namespace WSBInvestmentPredictor.Prediction.Domain.Enums;

/// <summary>
/// Specifies the mode of execution for the stock prediction engine.
/// </summary>
public enum RunMode
{
    /// <summary>
    /// Predicts the return for the most recent available data sample (real-time scenario).
    /// </summary>
    CurrentPrediction,

    /// <summary>
    /// Performs historical backtesting by training and predicting iteratively across a time range.
    /// </summary>
    HistoricalBacktest
}