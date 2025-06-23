namespace WSBInvestmentPredictor.Prediction.Shared.Dto;

/// <summary>
/// Data transfer object containing the results of a backtest operation.
/// Includes individual prediction points and overall performance metrics.
/// </summary>
public class BacktestResultDto
{
    /// <summary>
    /// Gets or sets the list of individual backtest points containing predicted and actual values.
    /// </summary>
    public List<BacktestPoint> Points { get; set; }

    /// <summary>
    /// Gets or sets the accuracy of the predictions as a percentage.
    /// </summary>
    public float Accuracy { get; set; }

    /// <summary>
    /// Gets or sets the mean squared error of the predictions.
    /// </summary>
    public float MeanSquaredError { get; set; }
}