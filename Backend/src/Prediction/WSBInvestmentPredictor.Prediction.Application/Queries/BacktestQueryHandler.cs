using MediatR;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;
using WSBInvestmentPredictor.Prediction.MarketData;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;

namespace WSBInvestmentPredictor.Prediction.Application.Queries;

public class BacktestQueryHandler : IRequestHandler<RunBacktestQuery, BacktestResultDto>
{
    private readonly IPolygonClient _polygonClient;
    private readonly IPredictionEngine _predictionEngine;

    public BacktestQueryHandler(IPolygonClient polygonClient, IPredictionEngine predictionEngine)
    {
        _polygonClient = polygonClient;
        _predictionEngine = predictionEngine;
    }

    public async Task<BacktestResultDto> Handle(RunBacktestQuery request, CancellationToken cancellationToken)
    {
        var points = new List<BacktestPoint>();
        var ticker = request.Ticker;
        var year = request.Year;

        // Zakres dat do pobrania danych
        var start = new DateTime(year, 1, 1);
        var end = new DateTime(year, 12, 31);

        // Pobierz dane OHLCV
        var allData = await _polygonClient.GetDailyOhlcvAsync(ticker, start.AddDays(-360), end.AddDays(60));

        // Co tydzień (poniedziałek) backtest
        var mondayDates = Enumerable.Range(0, 52)
            .Select(i => start.AddDays(i * 7))
            .Where(d => d <= end)
            .ToList();

        foreach (var predictionDate in mondayDates)
        {
            var inputData = allData
                .Where(d => DateTime.Parse(d.Date) < predictionDate)
                .OrderByDescending(d => DateTime.Parse(d.Date))
                .Take(360)
                .Reverse()
                .ToList();

            if (inputData.Count < 30)
                continue;

            var futureData = allData
                .FirstOrDefault(d => DateTime.Parse(d.Date) == predictionDate.AddDays(30));

            if (futureData == null)
                continue;

            var predicted = await _predictionEngine.PredictAsync(inputData);

            var actualChange = (futureData.Close - inputData.Last().Close) / inputData.Last().Close;

            points.Add(new BacktestPoint
            {
                Date = predictionDate,
                PredictedChange = predicted.ChangePercentage,
                ActualChange = actualChange
            });
        }

        var mse = points.Any()
            ? points.Average(p => MathF.Pow(p.PredictedChange - p.ActualChange, 2))
            : 0f;

        var accuracy = points.Any()
            ? points.Count(p => Math.Sign(p.PredictedChange) == Math.Sign(p.ActualChange)) / (float)points.Count
            : 0f;

        return new BacktestResultDto
        {
            Points = points,
            Accuracy = accuracy,
            MeanSquaredError = mse
        };
    }
}