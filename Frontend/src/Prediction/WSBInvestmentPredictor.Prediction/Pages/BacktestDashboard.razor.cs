using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Pages;

/// <summary>
/// Component for running and displaying backtest results.
/// Allows users to test the prediction model's performance on historical data.
/// </summary>
public class BacktestDashboardBase : ComponentBase
{
    [Inject] protected ICqrsRequestService Cqrs { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IStringLocalizer<SharedResource> Loc { get; set; } = default!;

    // List of available company tickers
    protected List<CompanyTicker> Tickers { get; set; } = new();

    // Selected company symbol
    protected string SelectedTicker { get; set; } = string.Empty;

    // Selected year for backtest
    protected int SelectedYear { get; set; } = DateTime.Now.Year;

    // Available years for backtest
    protected List<int> AvailableYears { get; set; } = new() { 2023, 2024 };

    // Backtest results
    protected BacktestResultDto? Result { get; set; }

    // Error message if backtest fails
    protected string? ErrorMessage { get; set; }

    // Loading state indicator
    protected bool IsLoading { get; set; }

    // Determines if backtest can be run
    protected bool CanRun => !string.IsNullOrWhiteSpace(SelectedTicker) && !IsLoading;

    /// <summary>
    /// Initializes the component by loading available tickers.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            Tickers = await Cqrs.Handle<GetSp500TickersQuery, List<CompanyTicker>>(new());
        }
        catch (Exception ex)
        {
            ErrorMessage = string.Format(Loc["ErrorLoadingTickers"], ex.Message);
        }
    }

    /// <summary>
    /// Runs the backtest for the selected company and year.
    /// </summary>
    protected async Task RunBacktest()
    {
        if (!CanRun) return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            Result = null;
            await InvokeAsync(StateHasChanged);

            Result = await Cqrs.Handle<RunBacktestQuery, BacktestResultDto>(
                new(SelectedTicker, SelectedYear));
        }
        catch (Exception ex)
        {
            ShowError(string.Format(Loc["ErrorRunningBacktest"], ex.Message));
        }
        finally
        {
            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// Shows an error notification to the user.
    /// </summary>
    private void ShowError(string message)
    {
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Error,
            Summary = "Error",
            Detail = message,
            Duration = 4000
        });
    }

    /// <summary>
    /// Generates diagonal line points for the scatter plot (ideal prediction line).
    /// </summary>
    /// <returns>List of points on the diagonal line</returns>
    protected List<DiagonalPoint> GetDiagonalLine()
    {
        var points = new List<DiagonalPoint>();
        for (float v = -0.1f; v <= 0.1f; v += 0.01f)
        {
            points.Add(new DiagonalPoint { Value = v });
        }
        return points;
    }

    /// <summary>
    /// Creates a histogram of prediction errors.
    /// </summary>
    /// <returns>List of histogram bins and their counts</returns>
    protected List<ErrorHistogramPoint> GetErrorHistogram()
    {
        if (Result?.Points == null || Result.Points.Count == 0)
            return new();

        var errors = Result.Points.Select(p => p.PredictedChange - p.ActualChange).ToList();
        var min = errors.Min();
        var max = errors.Max();
        int bins = 10;
        double binSize = (max - min) / bins;

        var histogram = new List<ErrorHistogramPoint>();
        for (int i = 0; i < bins; i++)
        {
            double binStart = min + i * binSize;
            double binEnd = binStart + binSize;
            int count = errors.Count(e => e >= binStart && e < binEnd);
            histogram.Add(new ErrorHistogramPoint
            {
                Range = $"{binStart:P2} - {binEnd:P2}",
                Count = count
            });
        }
        return histogram;
    }

    /// <summary>
    /// Exports backtest results to a CSV file.
    /// </summary>
    protected async Task ExportToCsv()
    {
        if (Result?.Points == null || !Result.Points.Any())
        {
            ShowError(Loc["NoDataToExport"]);
            return;
        }

        try
        {
            var csv = new StringBuilder();
            csv.AppendLine("Date,Prediction (%),Actual (%),Error (%)");

            foreach (var point in Result.Points)
            {
                var error = point.PredictedChange - point.ActualChange;
                csv.AppendLine($"{point.Date:yyyy-MM-dd},{point.PredictedChange:P2},{point.ActualChange:P2},{error:P2}");
            }

            var fileName = $"backtest_{SelectedTicker}_{SelectedYear}.csv";
            var csvString = csv.ToString();

            await JSRuntime.InvokeVoidAsync("eval", $@"
                var blob = new Blob([`{csvString.Replace("`", "\\``")}`], {{ type: 'text/csv' }});
                var link = document.createElement('a');
                link.href = URL.createObjectURL(blob);
                link.download = '{fileName}';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            ");
        }
        catch (Exception ex)
        {
            ShowError(string.Format(Loc["ErrorExporting"], ex.Message));
        }
    }

    /// <summary>
    /// Represents a point on the diagonal line in the scatter plot.
    /// </summary>
    protected class DiagonalPoint
    {
        public float Value { get; set; }
    }

    /// <summary>
    /// Represents a bin in the error histogram.
    /// </summary>
    protected class ErrorHistogramPoint
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}