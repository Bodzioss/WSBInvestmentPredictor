using MediatR;
using WSBInvestmentPredictor.Prediction.Application.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

public record PredictFromRawQuery(List<RawMarketData> RawData) : IRequest<PredictionResultDto>;