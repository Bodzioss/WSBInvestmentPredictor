using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WSBInvestmentPredictor.Prediction.Models;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using WSBInvestmentPredictor.Technology.Cqrs;

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
                new( "AAPL", "Apple Inc.")
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
            var rawData = await Cqrs.Handle<GetRawMarketDataQuery, List<RawMarketData>>(
                new GetRawMarketDataQuery(symbol, from, to)
            );

            if (rawData == null || rawData.Count < 1)
            {
                error = "Brak danych historycznych dla wybranego symbolu.";
                return;
            }

            var result = await Cqrs.Handle<PredictFromRawQuery, PredictionResultDto>(
                new PredictFromRawQuery(rawData)
            );

            prediction = result?.Prediction;
        }
        catch (Exception ex)
        {
            error = $"Błąd: {ex.Message}";
        }

        isLoading = false;
    }
}