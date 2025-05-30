using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Pages;

public class BacktestDashboardBase : ComponentBase
{
    [Inject] protected ICqrsRequestService Cqrs { get; set; } = default!;
    [Inject] NotificationService NotificationService { get; set; }

    protected List<CompanyTicker> Tickers { get; set; } = new();
    protected string SelectedTicker { get; set; } = string.Empty;
    protected int SelectedYear { get; set; } = DateTime.Now.Year;
    protected List<int> AvailableYears { get; set; } = new() { 2023, 2024 };

    RadzenNotification notification;

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

        IsLoading = true;
        ErrorMessage = null;
        Result = null;

        try
        {
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
}