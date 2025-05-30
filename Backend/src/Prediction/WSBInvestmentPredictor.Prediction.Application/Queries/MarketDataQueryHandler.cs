using MediatR;
using WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;
using WSBInvestmentPredictor.Prediction.MarketData;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;

namespace WSBInvestmentPredictor.Prediction.Application.Queries;

public class MarketDataQueryHandler : IRequestHandler<GetSp500TickersQuery, List<CompanyTicker>>
{
    private readonly ISp500TickerProvider _provider;
    private readonly IPolygonClient _polygon;

    public MarketDataQueryHandler(ISp500TickerProvider provider, IPolygonClient polygon)
    {
        _provider = provider;
        _polygon = polygon;
    }

    public Task<List<CompanyTicker>> Handle(GetSp500TickersQuery request, CancellationToken cancellationToken)
    {
        var result = _provider.GetAll().ToList();
        return Task.FromResult(result);
    }

    public async Task<List<RawMarketData>> Handle(GetRawMarketDataQuery request, CancellationToken cancellationToken)
    {
        var data = await _polygon.GetDailyOhlcvAsync(request.Symbol, request.From, request.To);
        return data;
    }
}