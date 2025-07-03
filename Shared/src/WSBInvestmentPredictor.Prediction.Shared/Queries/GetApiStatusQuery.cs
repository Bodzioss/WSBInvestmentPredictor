using MediatR;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Prediction.Shared.Queries;

/// <summary>
/// Query for retrieving the current status of the prediction API.
/// Returns information about the API's operational status, version, and timestamp.
/// </summary>
[ApiRequest("/api/status", "GET")]
public record GetApiStatusQuery : IRequest<ApiStatusDto>;