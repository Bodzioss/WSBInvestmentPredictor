using MediatR;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;

namespace WSBInvestmentPredictor.Prediction.Application.Commands;

public class PredictionCommandHandler : IRequest
{
    private readonly IStockPredictorService _predictor;

    public PredictionCommandHandler(IStockPredictorService predictor)
    {
        _predictor = predictor;
    }

    public Task<Unit> Handle(TrainModelCommand request, CancellationToken cancellationToken)
    {
        _predictor.Train(request.Data);
        return Task.FromResult(Unit.Value);
    }
}