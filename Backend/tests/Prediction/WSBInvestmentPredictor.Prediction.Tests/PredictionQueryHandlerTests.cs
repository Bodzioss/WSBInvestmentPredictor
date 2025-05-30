using WSBInvestmentPredictor.Prediction.Application.FeatureEngeneering;
using WSBInvestmentPredictor.Prediction.Application.Queries;
using WSBInvestmentPredictor.Prediction.Shared.Queries;
using WSBInvestmentPredictor.Prediction.UnitTests.Builders;
using WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.UnitTests;

public class PredictionQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPredictionFromFakeService()
    {
        // Arrange
        var builder = new MarketDataPredictionBuilder();
        var predictor = new TestStockPredictorService { PredictionToReturn = 0.123f };
        var handler = new PredictionQueryHandler(predictor, builder);
        var input = new MarketDataBuilder().Build();
        var query = new GetPredictionQuery(input);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(0.123f, result.Prediction);
    }
}