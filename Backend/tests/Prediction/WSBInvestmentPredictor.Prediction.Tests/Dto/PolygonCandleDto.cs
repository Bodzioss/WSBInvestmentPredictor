using System.Text.Json.Serialization;

namespace WSBInvestmentPredictor.Prediction.UnitTests.Dto;

public class PolygonCandleDto
{
    [JsonPropertyName("t")]
    public long t { get; set; } // Timestamp w milisekundach od Unix Epoch

    [JsonPropertyName("o")]
    public decimal o { get; set; } // Open

    [JsonPropertyName("h")]
    public decimal h { get; set; } // High

    [JsonPropertyName("l")]
    public decimal l { get; set; } // Low

    [JsonPropertyName("c")]
    public decimal c { get; set; } // Close

    [JsonPropertyName("v")]
    public decimal v { get; set; } // Volume
}

public class PolygonCandleResponseDto
{
    [JsonPropertyName("results")]
    public List<PolygonCandleDto> results { get; set; } = new List<PolygonCandleDto>();
}