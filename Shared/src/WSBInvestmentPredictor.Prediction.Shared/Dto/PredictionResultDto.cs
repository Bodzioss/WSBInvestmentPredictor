namespace WSBInvestmentPredictor.Prediction.Application.Dto;

/// <summary>
/// DTO representing the output of a prediction query.
/// Contains the predicted price change percentage for a given stock.
/// </summary>
/// <param name="Prediction">The predicted price change percentage.</param>
public record PredictionResultDto(float Prediction);