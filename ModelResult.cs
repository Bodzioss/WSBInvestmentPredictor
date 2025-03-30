namespace WSBInvestmentPredictor
{
    /// <summary>
    /// Przechowuje wynik modelu dla jednej spółki
    /// </summary>
    public class ModelResult
    {
        /// <summary>Ticker spółki (nazwa pliku bez rozszerzenia)</summary>
        public string Ticker { get; set; }

        /// <summary>R-squared – jakość dopasowania modelu (0–1, im wyżej tym lepiej)</summary>
        public float RSquared { get; set; }

        /// <summary>Średni błąd bezwzględny prognozy zwrotu</summary>
        public float MAE { get; set; }

        /// <summary>Błąd średniokwadratowy – uwzględnia duże błędy</summary>
        public float RMSE { get; set; }

        /// <summary>Prognozowany zwrot 30-dniowy (%) na podstawie ostatnich danych</summary>
        public float Prediction { get; set; }

        /// <summary>Liczba wierszy (dni) danych rynkowych</summary>
        public int Rows { get; set; }

        /// <summary>
        /// Metryka rankingowa: im wyższa, tym lepsze połączenie przewidywanego zysku i jakości modelu
        /// </summary>
        public float RankingScore => (RSquared * Prediction) / (MAE + RMSE + 1e-6f); // +1e-6 by uniknąć dzielenia przez 0
    }
}