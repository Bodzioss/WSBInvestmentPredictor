using MediatR;
using WSBInvestmentPredictor.Prediction.Application.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

/// <summary>
/// Query for generating a prediction from raw market data.
/// Takes a list of raw market data points and returns a prediction result.
/// </summary>
/// <param name="RawData">The list of raw market data points to use for prediction.</param>
[ApiRequest("/api/Prediction/predict-from-raw", "POST")]
public record PredictFromRawQuery(List<RawMarketData> RawData) : IRequest<PredictionResultDto>;