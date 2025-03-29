using Microsoft.ML.Data;

namespace AIInvestmentPredictor
{
    public class MarketData
    {
        // Właściwości wejściowe (cechy modelu)
        [LoadColumn(1)] public float Open { get; set; }
        [LoadColumn(2)] public float High { get; set; }
        [LoadColumn(3)] public float Low { get; set; }
        [LoadColumn(4)] public float Close { get; set; }
        [LoadColumn(5)] public float AdjClose { get; set; }
        [LoadColumn(6)] public float Volume { get; set; }

        // Zmienna docelowa (np. przyszła cena)
        // Można np. prognozować Close z przesunięciem (t+5, t+30 itd.)
        [LoadColumn(7)] public float Target { get; set; }
    }
}
