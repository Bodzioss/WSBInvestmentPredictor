namespace WSBInvestmentPredictor.Prediction.Shared.Dto;

public class BacktestPoint
{
    public DateTime Date { get; set; }
    public float PredictedChange { get; set; }
    public float ActualChange { get; set; }
}