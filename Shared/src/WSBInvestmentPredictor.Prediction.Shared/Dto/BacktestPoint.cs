namespace WSBInvestmentPredictor.Prediction.Shared.Dto;

/// <summary>
/// Represents a single data point in a backtest result.
/// Contains both predicted and actual price changes for a specific date.
/// </summary>
public class BacktestPoint
{
    /// <summary>
    /// Gets or sets the date of the backtest point.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the predicted price change percentage.
    /// </summary>
    public float PredictedChange { get; set; }

    /// <summary>
    /// Gets or sets the actual price change percentage that occurred.
    /// </summary>
    public float ActualChange { get; set; }

    /// <summary>
    /// Gets the date formatted as a string in "yyyy-MM-dd" format.
    /// </summary>
    public string DateString => Date.ToString("yyyy-MM-dd");
}