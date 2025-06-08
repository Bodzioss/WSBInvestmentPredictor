using System.Net;
using WSBInvestmentPredictor.Prediction.Application.Queries;
using WSBInvestmentPredictor.Prediction.Infrastructure;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using WSBInvestmentPredictor.Prediction.UnitTests.Builders;
using WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.API.Tests.Queries;

public class GetApiStatusQueryTests
{
    [Fact]
    public async Task GetApiStatus_ShouldReturnOk()
    {
        // Act
        var handler = new ApiStatusQueryHandler();
        var query = new GetApiStatusQuery();
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OK", result.Status);
        Assert.Equal("1.0.0", result.Version);
        Assert.True(result.Timestamp <= DateTime.UtcNow);
    }
} 
