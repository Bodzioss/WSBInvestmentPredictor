using MediatR;
using WSBInvestmentPredictor.Prediction.Application.Dto;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;

namespace WSBInvestmentPredictor.Prediction.Application.Queries;

public class PredictionQueryHandler : IRequestHandler<GetPredictionQuery, PredictionResultDto>
{
    private readonly IStockPredictorService _predictor;

    public PredictionQueryHandler(IStockPredictorService predictor)
    {
        _predictor = predictor;
    }

    public Task<PredictionResultDto> Handle(GetPredictionQuery request, CancellationToken cancellationToken)
    {
        var score = _predictor.Predict(request.Sample);
        return Task.FromResult(new PredictionResultDto(score));
    }
}