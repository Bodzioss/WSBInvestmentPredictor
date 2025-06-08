namespace WSBInvestmentPredictor.Prediction.Shared.Dto;

public record ApiStatusDto(
    string Status,
    string Version,
    DateTime Timestamp
); 