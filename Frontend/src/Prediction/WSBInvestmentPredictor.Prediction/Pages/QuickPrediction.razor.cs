using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
using WSBInvestmentPredictor.Prediction.Models;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Prediction.Application.Queries;

namespace WSBInvestmentPredictor.Prediction.Pages;

public partial class QuickPrediction : ComponentBase
{
    [Inject] protected HttpClient Http { get; set; } = default!;

    protected List<CompanyTicker> availableTickers = [];
    protected string symbol = string.Empty;
    protected List<int> dayOptions = [30, 60, 100, 150];
    protected int selectedDays = 100;
    protected string searchTerm = string.Empty;

    protected IEnumerable<CompanyTicker> FilteredTickers =>
        availableTickers
            .Where(t => string.IsNullOrEmpty(searchTerm) ||
                        t.Ticker.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));


    protected bool isLoading = false;
    protected string error;
    protected float? prediction;

    [Inject] protected ICqrsRequestService Cqrs { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            availableTickers = await Cqrs.Handle<GetSp500TickersQuery, List<CompanyTicker>>(new())
                                ?? [];
             
            symbol = availableTickers.FirstOrDefault()?.Ticker ?? "AAPL";
        }
        catch (Exception ex)
        {
            error = $"Nie udało się pobrać listy tickerów: {ex.Message}";
            availableTickers = [
                new() { Ticker = "AAPL", Name = "Apple Inc." }
            ];
            symbol = "AAPL";
        }
    }

    protected async Task PredictAsync()
    {
        isLoading = true;
        error = null;
        prediction = null;

        var to = DateTime.UtcNow.Date;
        var from = to.AddDays(-selectedDays);

        try
        {
            var rawData = await Http.GetFromJsonAsync<List<RawMarketData>>(
                $"/api/marketdata/{symbol}?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}"
            );

            if (rawData == null || rawData.Count < 1)
            {
                error = "Brak danych historycznych dla wybranego symbolu.";
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
