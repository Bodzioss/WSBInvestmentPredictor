namespace WSBInvestmentPredictor.Prediction.Models;

public class RawMarketData
{
    public string Date { get; set; } = string.Empty;
    public float Open { get; set; }
    public float High { get; set; }
    public float Low { get; set; }
    public float Close { get; set; }
    public float Volume { get; set; }
}