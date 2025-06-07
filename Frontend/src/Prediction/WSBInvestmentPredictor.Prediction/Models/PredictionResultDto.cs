namespace WSBInvestmentPredictor.Prediction.Models;

/// <summary>
/// Represents the response from the Prediction API containing the predicted return.
/// </summary>
public class PredictionResultDto
{
    public float Prediction { get; set; }
}