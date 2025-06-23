using MediatR;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

/// <summary>
/// Query for retrieving the list of S&P 500 company tickers and names.
/// Returns a list of company information including their stock symbols and full names.
/// </summary>
[ApiRequest("/api/MarketData/tickers", "GET")]
public class GetSp500TickersQuery : IRequest<List<CompanyTicker>>;