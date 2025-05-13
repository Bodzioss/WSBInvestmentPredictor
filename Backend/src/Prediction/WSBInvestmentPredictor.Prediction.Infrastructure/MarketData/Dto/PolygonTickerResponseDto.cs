using System.Text.Json.Serialization;

namespace WSBInvestmentPredictor.Prediction.Infrastructure.MarketData.Dto;

public class PolygonTickerResponseDto
{
    [JsonPropertyName("results")]
    public List<PolygonTickerDto> Results { get; set; } = [];

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}