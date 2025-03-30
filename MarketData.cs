using Microsoft.ML.Data;

namespace WSBInvestmentPredictor
{
    /// <summary>
    /// Reprezentuje jedną obserwację rynkową (np. jeden dzień notowań) używaną do treningu modelu.
    /// Zawiera dane wejściowe (cechy) oraz wartość docelową (Target).
    /// </summary>
    public class MarketData
    {
        [LoadColumn(0)]
        public string Date { get; set; } // Format: "yyyy-MM-dd"

        [LoadColumn(1)] public float Open { get; set; }
        [LoadColumn(2)] public float High { get; set; }
        [LoadColumn(3)] public float Low { get; set; }
        [LoadColumn(4)] public float Close { get; set; }
        [LoadColumn(5)] public float Volume { get; set; }
        [LoadColumn(6)] public float SMA_5 { get; set; }
        [LoadColumn(7)] public float SMA_10 { get; set; }
        [LoadColumn(8)] public float SMA_20 { get; set; }
        [LoadColumn(9)] public float Volatility_10 { get; set; }
        [LoadColumn(10)] public float RSI_14 { get; set; }
        [LoadColumn(11)] public float Target { get; set; }
    }
}
