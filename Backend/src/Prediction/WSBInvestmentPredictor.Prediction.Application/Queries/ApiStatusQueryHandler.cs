using MediatR;
using WSBInvestmentPredictor.Prediction.Shared.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Queries;

namespace WSBInvestmentPredictor.Prediction.Application.Queries;

public class ApiStatusQueryHandler : IRequestHandler<GetApiStatusQuery, ApiStatusDto>
{
    public Task<ApiStatusDto> Handle(GetApiStatusQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ApiStatusDto(
            Status: "OK",
            Version: "1.0.0",
            Timestamp: DateTime.UtcNow
        ));
    }
} 