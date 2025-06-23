using MediatR;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

/// <summary>
/// Query for retrieving raw market data for a specific stock symbol within a date range.
/// Returns a list of OHLCV (Open, High, Low, Close, Volume) data points.
/// </summary>
/// <param name="Symbol">The stock symbol to retrieve data for.</param>
/// <param name="From">The start date of the data range.</param>
/// <param name="To">The end date of the data range.</param>
[ApiRequest("/api/Prediction/get-raw-market-data", "POST")]
public record GetRawMarketDataQuery(string Symbol, DateTime From, DateTime To)
    : IRequest<List<RawMarketData>>;