using MediatR;
using WSBInvestmentPredictor.Prediction.Application.Dto;
using WSBInvestmentPredictor.Prediction.Application.FeatureEngeneering;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;
using WSBInvestmentPredictor.Prediction.Shared.Queries;

namespace WSBInvestmentPredictor.Prediction.Application.Queries;

public class PredictionQueryHandler 
    : IRequestHandler<GetPredictionQuery, PredictionResultDto>,
      IRequestHandler<PredictFromRawQuery, PredictionResultDto>
{
    private readonly IStockPredictorService _predictor;
    private readonly MarketDataPredictionBuilder _builder;

    public PredictionQueryHandler(IStockPredictorService predictor, MarketDataPredictionBuilder builder)
    {
        _predictor = predictor;
        _builder = builder;
    }

    public Task<PredictionResultDto> Handle(GetPredictionQuery request, CancellationToken cancellationToken)
    {
        var score = _predictor.Predict(request.Sample);
        return Task.FromResult(new PredictionResultDto(score));
    }

    public Task<PredictionResultDto> Handle(PredictFromRawQuery request, CancellationToken cancellationToken)
    {
        var processed = _builder.Build(request.RawData);
        if (processed.Count == 0)
            throw new ArgumentException("Insufficient data for feature engineering.");

        _predictor.Train(processed);
        var result = _predictor.Predict(processed.Last());

        return Task.FromResult(new PredictionResultDto(result));
    }
}