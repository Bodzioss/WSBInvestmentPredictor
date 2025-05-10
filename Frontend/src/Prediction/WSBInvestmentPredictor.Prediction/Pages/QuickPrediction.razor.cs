using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WSBInvestmentPredictor.Prediction.Pages;

public partial class QuickPrediction : ComponentBase
{
    [Inject] protected HttpClient Http { get; set; } = default!;

    protected string symbol = "AAPL";
    protected bool isLoading = false;
    protected string error;
    protected float? prediction;

    protected async Task PredictAsync()
    {
        isLoading = true;
        error = null;
        prediction = null;

        try
        {
            var rawData = await Http.GetFromJsonAsync<List<RawMarketData>>($"/api/MarketData/{symbol}");

            if (rawData == null || rawData.Count < 1)
            {
                error = "Brak danych historycznych dla symbolu.";
                return;
            }

            var response = await Http.PostAsJsonAsync("/api/Prediction/predict-from-raw", rawData);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                prediction = json?.prediction;
            }
            else
            {
                var msg = await response.Content.ReadAsStringAsync();
                error = $"Błąd API: {msg}";
            }
        }
        catch (Exception ex)
        {
            error = $"Błąd: {ex.Message}";
        }

        isLoading = false;
    }

    public class RawMarketData
    {
        public string Date { get; set; } = string.Empty;
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public float Volume { get; set; }
    }

    public class PredictionResponse
    {
        public float prediction { get; set; }
    }
}