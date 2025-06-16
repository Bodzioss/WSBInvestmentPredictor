namespace WSBInvestmentPredictor.Prediction.Shared.Dto;
/// <summary>
/// Basic OHLCV data used as input before feature engineering.
/// Represents raw market data with Open, High, Low, Close prices and Volume.
/// </summary>
/// <param name="Date">The date of the market data in string format.</param>
/// <param name="Open">The opening price of the trading period.</param>
/// <param name="High">The highest price reached during the trading period.</param>
/// <param name="Low">The lowest price reached during the trading period.</param>
/// <param name="Close">The closing price of the trading period.</param>
/// <param name="Volume">The trading volume for the period.</param>
public record RawMarketData(
    string Date,
    float Open,
    float High,
    float Low,
    float Close,
    float Volume
);