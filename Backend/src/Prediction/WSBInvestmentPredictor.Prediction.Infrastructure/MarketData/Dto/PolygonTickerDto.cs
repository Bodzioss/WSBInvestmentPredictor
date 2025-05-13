using System.Text.Json.Serialization;

namespace WSBInvestmentPredictor.Prediction.Infrastructure.MarketData.Dto;

public class PolygonTickerDto
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}