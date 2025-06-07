using MediatR;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;
[ApiRequest("/api/backtest", "POST")]
public record RunBacktestQuery(string Ticker, int Year) : IRequest<BacktestResultDto>;