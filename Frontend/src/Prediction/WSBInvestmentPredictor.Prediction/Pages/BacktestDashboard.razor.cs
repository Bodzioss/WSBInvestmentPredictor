using Microsoft.AspNetCore.Components;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using WSBInvestmentPredictor.Prediction.Shared.Queries;

namespace WSBInvestmentPredictor.Prediction.Pages;

public class BacktestDashboardBase : ComponentBase
{
    [Inject] protected ICqrsRequestService Cqrs { get; set; }

    protected List<CompanyTicker> Tickers { get; set; } = new();
    protected string SelectedTicker { get; set; }
    protected int SelectedYear { get; set; } = DateTime.Now.Year;
    protected List<int> AvailableYears { get; set; } = new() { 2023, 2024 };

    protected BacktestResultDto Result { get; set; }

    protected bool CanRun => !string.IsNullOrWhiteSpace(SelectedTicker);

    protected override async Task OnInitializedAsync()
    {
        Tickers = await Cqrs.Handle<GetSp500TickersQuery, List<CompanyTicker>>(new());
    }

    protected async Task RunBacktest()
    {
        Result = await Cqrs.Handle<RunBacktestQuery, BacktestResultDto>(
            new(SelectedTicker, SelectedYear));
    }
}
