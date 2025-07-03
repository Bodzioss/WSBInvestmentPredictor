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

/// <summary>
/// Component for making quick predictions based on historical market data.
/// Allows users to select a company and time period to get predictions.
/// </summary>
public partial class QuickPrediction : ComponentBase
{
    [Inject] protected HttpClient Http { get; set; } = default!;

    // List of available company tickers
    protected List<CompanyTicker> availableTickers = [];

    // Selected company symbol
    protected string symbol = string.Empty;

    // Available time period options for historical data
    protected List<int> dayOptions = [30, 60, 100, 150];

    // Selected number of days for historical data
    protected int selectedDays = 100;

    // Search term for filtering companies
    protected string searchTerm = string.Empty;

    /// <summary>
    /// Filters the available tickers based on the search term.
    /// </summary>
    protected IEnumerable<CompanyTicker> FilteredTickers =>
        availableTickers
            .Where(t => string.IsNullOrEmpty(searchTerm) ||
                        t.Ticker.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

    // Loading state indicator
    protected bool isLoading = false;

    // Error message if prediction fails
    protected string error;

    // Prediction result
    protected float? prediction;

    [Inject] protected ICqrsRequestService Cqrs { get; set; } = default!;

    /// <summary>
    /// Initializes the component by loading available tickers.
    /// </summary>
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
            error = $"Failed to load ticker list: {ex.Message}";
            availableTickers = [
                new( "AAPL", "Apple Inc.")
            ];
            symbol = "AAPL";
        }
    }

    /// <summary>
    /// Makes a prediction based on historical data for the selected company and time period.
    /// </summary>
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
                error = "No historical data available for the selected symbol.";
                return;
            }

            var result = await Cqrs.Handle<PredictFromRawQuery, PredictionResultDto>(
                new PredictFromRawQuery(rawData)
            );

            prediction = result?.Prediction;
        }
        catch (Exception ex)
        {
            error = $"Error: {ex.Message}";
        }

        isLoading = false;
    }
}