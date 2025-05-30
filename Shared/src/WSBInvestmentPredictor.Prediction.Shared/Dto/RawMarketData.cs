namespace WSBInvestmentPredictor.Prediction.Shared.Dto;
/// <summary>
/// Basic OHLCV data used as input before feature engineering.
/// </summary>
public record RawMarketData(
    string Date,
    float Open,
    float High,
    float Low,
    float Close,
    float Volume
);