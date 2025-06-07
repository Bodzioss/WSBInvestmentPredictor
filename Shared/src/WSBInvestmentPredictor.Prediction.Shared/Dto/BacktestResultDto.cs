namespace WSBInvestmentPredictor.Prediction.Shared.Dto;

public class BacktestResultDto
{
    public List<BacktestPoint> Points { get; set; }
    public float Accuracy { get; set; }
    public float MeanSquaredError { get; set; }
}