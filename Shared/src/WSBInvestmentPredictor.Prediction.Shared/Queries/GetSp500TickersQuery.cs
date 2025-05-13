using MediatR;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

[ApiRequest("/api/marketdata/tickers", "GET")]
public class GetSp500TickersQuery : IRequest<List<CompanyTicker>>;