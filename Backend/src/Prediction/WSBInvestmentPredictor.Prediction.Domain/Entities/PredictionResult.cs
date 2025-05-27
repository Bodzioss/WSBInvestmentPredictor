namespace WSBInvestmentPredictor.Prediction.Domain.Entities;

/// <summary>
/// The result of a machine learning prediction.
/// </summary>
/// <param name="Score">Predicted return (regression output), typically expressed as a float between -1 and 1.</param>
public class PredictionResult
{
    public float Score { get; set; }
    public float ChangePercentage { get; set; }
}