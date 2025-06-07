using MediatR;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;
[ApiRequest("/api/Prediction/get-raw-market-data", "POST")]
public record GetRawMarketDataQuery(string Symbol, DateTime From, DateTime To)
    : IRequest<List<RawMarketData>>;