using MediatR;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

/// <summary>
/// Query for running a backtest on historical data for a specific stock.
/// Returns detailed backtest results including prediction accuracy and individual data points.
/// </summary>
/// <param name="Ticker">The stock symbol to run the backtest for.</param>
/// <param name="Year">The year to use for the backtest period.</param>
[ApiRequest("/api/backtest", "POST")]
public record RunBacktestQuery(string Ticker, int Year) : IRequest<BacktestResultDto>;