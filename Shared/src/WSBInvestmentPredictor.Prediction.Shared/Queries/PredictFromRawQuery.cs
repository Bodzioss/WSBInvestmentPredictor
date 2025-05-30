using MediatR;
using WSBInvestmentPredictor.Prediction.Application.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

[ApiRequest("/api/Prediction/predict-from-raw", "POST")]
public record PredictFromRawQuery(List<RawMarketData> RawData) : IRequest<PredictionResultDto>;