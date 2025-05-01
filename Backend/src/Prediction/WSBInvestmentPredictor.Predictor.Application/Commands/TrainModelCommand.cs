using MediatR;
using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.Application.Commands;

/// <summary>
/// Command to train the prediction model on historical data.
/// </summary>
public record TrainModelCommand(List<MarketData> Data) : IRequest;