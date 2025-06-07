namespace WSBInvestmentPredictor.Prediction.Domain.Entities;

/// <summary>
/// Represents a single row of market data used for training and prediction.
/// Contains raw price information, technical indicators, and the prediction target.
/// </summary>
/// <param name="Date">The date of the observation (format: yyyy-MM-dd).</param>
/// <param name="Open">Opening price of the stock.</param>
/// <param name="High">Highest price during the trading day.</param>
/// <param name="Low">Lowest price during the trading day.</param>
/// <param name="Close">Closing price of the stock.</param>
/// <param name="Volume">Number of shares traded.</param>
/// <param name="SMA_5">5-day Simple Moving Average.</param>
/// <param name="SMA_10">10-day Simple Moving Average.</param>
/// <param name="SMA_20">20-day Simple Moving Average.</param>
/// <param name="Volatility_10">10-day price volatility indicator.</param>
/// <param name="RSI_14">14-day Relative Strength Index.</param>
/// <param name="Target">Future return after 30 days, used as the label for prediction.</param>
public record MarketDataInput(
    string Date,
    float Open,
    float High,
    float Low,
    float Close,
    float Volume,
    float SMA_5,
    float SMA_10,
    float SMA_20,
    float Volatility_10,
    float RSI_14,
    float Target
);