using MediatR;
using WSBInvestmentPredictor.Prediction.InternalShared.ValueObjects;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Application.Queries;
[ApiRequest("/api/marketdata/tickers", "GET")]
public class GetSp500TickersQuery : IRequest<List<CompanyTicker>>;