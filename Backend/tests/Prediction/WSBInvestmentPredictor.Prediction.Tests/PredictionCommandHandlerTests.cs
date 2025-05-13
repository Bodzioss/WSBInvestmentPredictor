using WSBInvestmentPredictor.Prediction.Application.Commands;
using WSBInvestmentPredictor.Prediction.UnitTests.Builders;
using WSBInvestmentPredictor.Prediction.UnitTests.TestDoubles;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.UnitTests;

public class PredictionCommandHandlerTests
{
    [Fact]
    public async Task Handle_StoresTrainingCall()
    {
        // Arrange
        var predictor = new TestStockPredictorService();
        var handler = new PredictionCommandHandler(predictor);
        var data = new MarketDataBuilder().BuildMany(50);
        var command = new TrainModelCommand(data);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(predictor.WasTrained);
        Assert.Equal(50, predictor.TrainedData!.Count());
    }
}