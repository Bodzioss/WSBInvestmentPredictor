using WSBInvestmentPredictor.Prediction.Domain.Entities;

namespace WSBInvestmentPredictor.Prediction.Domain.Interfaces;

/// <summary>
/// Provides an interface for training a machine learning model using historical market data.
/// </summary>
public interface IModelTrainer
{
    /// <summary>
    /// Trains the model using the specified historical market data.
    /// </summary>
    /// <param name="data">Enumerable set of market data used for training.</param>
    void Train(IEnumerable<MarketDataInput> data);
}