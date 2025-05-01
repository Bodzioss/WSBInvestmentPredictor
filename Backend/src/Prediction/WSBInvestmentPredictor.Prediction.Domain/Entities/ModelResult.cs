namespace WSBInvestmentPredictor.Prediction.Domain.Entities;

/// <summary>
/// Holds evaluation metrics and prediction summary for a given stock.
/// </summary>
/// <param name="Ticker">Ticker symbol of the stock.</param>
/// <param name="RSquared">Coefficient of determination (R²) indicating model fit quality.</param>
/// <param name="MAE">Mean Absolute Error of the model.</param>
/// <param name="RMSE">Root Mean Squared Error of the model.</param>
/// <param name="Prediction">Predicted return for the most recent sample.</param>
/// <param name="Rows">Number of rows (data points) used for training and evaluation.</param>
public record ModelResult(
    string Ticker,
    float RSquared,
    float MAE,
    float RMSE,
    float Prediction,
    int Rows
);