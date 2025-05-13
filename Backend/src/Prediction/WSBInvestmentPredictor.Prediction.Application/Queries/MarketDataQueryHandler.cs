using MediatR;
using WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;

namespace WSBInvestmentPredictor.Prediction.Application.Queries;

public class MarketDataQueryHandler : IRequestHandler<GetSp500TickersQuery, List<CompanyTicker>>
{
    private readonly ISp500TickerProvider _provider;

    public MarketDataQueryHandler(ISp500TickerProvider provider)
    {
        _provider = provider;
    }

    public Task<List<CompanyTicker>> Handle(GetSp500TickersQuery request, CancellationToken cancellationToken)
    {
        var result = _provider.GetAll().ToList();
        return Task.FromResult(result);
    }
}