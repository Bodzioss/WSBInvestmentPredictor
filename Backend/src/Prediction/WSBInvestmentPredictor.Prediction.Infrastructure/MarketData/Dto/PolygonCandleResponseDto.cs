namespace WSBInvestmentPredictor.Prediction.Infrastructure.MarketData.Dto;

public class PolygonCandleResponseDto
{
    public string ticker { get; set; } = string.Empty;
    public List<PolygonBar> results { get; set; } = new();
    public string status { get; set; } = string.Empty;
}

public class PolygonBar
{
    public long t { get; set; } // timestamp
    public float o { get; set; } // open
    public float h { get; set; } // high
    public float l { get; set; } // low
    public float c { get; set; } // close
    public float v { get; set; } // volume
}