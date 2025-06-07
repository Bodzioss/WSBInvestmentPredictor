using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using WSBInvestmentPredictor.Technology.Cqrs;
using System.Text;
using Microsoft.JSInterop;

namespace WSBInvestmentPredictor.Prediction.Pages;

public class BacktestDashboardBase : ComponentBase
{
    [Inject] protected ICqrsRequestService Cqrs { get; set; } = default!;
    [Inject] NotificationService NotificationService { get; set; } = default!;
    [Inject] IJSRuntime JSRuntime { get; set; } = default!;

    protected List<CompanyTicker> Tickers { get; set; } = new();
    protected string SelectedTicker { get; set; } = string.Empty;
    protected int SelectedYear { get; set; } = DateTime.Now.Year;
    protected List<int> AvailableYears { get; set; } = new() { 2023, 2024 };

    protected BacktestResultDto? Result { get; set; }
    protected string? ErrorMessage { get; set; }
    protected bool IsLoading { get; set; }

    protected bool CanRun => !string.IsNullOrWhiteSpace(SelectedTicker) && !IsLoading;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Tickers = await Cqrs.Handle<GetSp500TickersQuery, List<CompanyTicker>>(new());
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd podczas pobierania tickerów: {ex.Message}";
        }
    }

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
            ShowError($"Błąd podczas uruchamiania backtestu: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    void ShowError(string message)
    {
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Error,
            Summary = "Błąd",
            Detail = message,
            Duration = 4000
        });
    }

    /// <summary>
    /// Generuje linię diagonalną y=x do wykresu punktowego (idealna linia predykcji).
    /// </summary>
    /// <returns>Lista punktów na linii diagonalnej</returns>
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
    /// Tworzy histogram błędów predykcji.
    /// </summary>
    /// <returns>Lista przedziałów histogramu i liczba punktów w każdym</returns>
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

    protected async Task ExportToCsv()
    {
        if (Result?.Points == null || !Result.Points.Any())
        {
            ShowError("Brak danych do eksportu");
            return;
        }

        try
        {
            var csv = new StringBuilder();
            csv.AppendLine("Data,Predykcja (%),Rzeczywistość (%),Błąd (%)");

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
            ShowError($"Błąd podczas eksportu: {ex.Message}");
        }
    }

    protected class DiagonalPoint
    {
        public float Value { get; set; }
    }

    protected class ErrorHistogramPoint
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
