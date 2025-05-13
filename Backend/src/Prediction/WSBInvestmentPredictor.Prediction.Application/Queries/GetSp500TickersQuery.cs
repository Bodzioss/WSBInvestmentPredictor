using MediatR;
using WSBInvestmentPredictor.Prediction.InternalShared.ValueObjects;

namespace WSBInvestmentPredictor.Prediction.Application.Queries;

public class GetSp500TickersQuery : IRequest<List<CompanyTicker>>;