using MediatR;
using WSBInvestmentPredictor.Prediction.Application.Dto;
using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.Application.Queries;

/// <summary>
/// Query to get a prediction for a given MarketData sample.
/// </summary>
public record GetPredictionQuery(MarketDataInput Sample) : IRequest<PredictionResultDto>;