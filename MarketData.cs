using Microsoft.ML.Data;

namespace WSBInvestmentPredictor
{
    /// <summary>
    /// Reprezentuje jedną obserwację rynkową (np. jeden dzień notowań) używaną do treningu modelu.
    /// Zawiera dane wejściowe (cechy) oraz wartość docelową (Target).
    /// </summary>
    public class MarketData
    {
        // 📈 Dane rynkowe (cechy podstawowe)

        [LoadColumn(0)]
        public float Open { get; set; } // Cena otwarcia w danym dniu

        [LoadColumn(1)]
        public float High { get; set; } // Najwyższa cena osiągnięta w danym dniu

        [LoadColumn(2)]
        public float Low { get; set; } // Najniższa cena w danym dniu

        [LoadColumn(3)]
        public float Close { get; set; } // Cena zamknięcia (końcowa cena dnia)

        [LoadColumn(4)]
        public float Volume { get; set; } // Liczba akcji wymienionych w danym dniu (wolumen)

        // Wskaźniki techniczne (cechy wyprowadzone z danych historycznych)

        [LoadColumn(5)]
        public float SMA_5 { get; set; } // Średnia ruchoma z 5 dni (Simple Moving Average)

        [LoadColumn(6)]
        public float SMA_10 { get; set; } // Średnia ruchoma z 10 dni

        [LoadColumn(7)]
        public float SMA_20 { get; set; } // Średnia ruchoma z 20 dni

        [LoadColumn(8)]
        public float Volatility_10 { get; set; } // Zmienność: odchylenie standardowe z 10 dni

        [LoadColumn(9)]
        public float RSI_14 { get; set; } // RSI (Relative Strength Index) z 14 dni – wskaźnik momentum

        // Wartość docelowa (label) – to, co model ma przewidzieć

        [LoadColumn(10)]
        public float Target { get; set; } // Zwrot procentowy po 30 dniach względem ceny Close z bieżącego dnia
    }
}
