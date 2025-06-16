using MediatR;
using WSBInvestmentPredictor.Prediction.Application.Dto;
using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

/// <summary>
/// Query to get a prediction for a given MarketData sample.
/// Returns a prediction result containing the expected price change percentage.
/// </summary>
/// <param name="Sample">The market data input containing price information and technical indicators.</param>
public record GetPredictionQuery(MarketDataInput Sample) : IRequest<PredictionResultDto>;