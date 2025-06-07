using MediatR;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

public record GetRawMarketDataQuery(string Symbol, DateTime From, DateTime To)
    : IRequest<List<RawMarketData>>;