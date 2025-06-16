namespace WSBInvestmentPredictor.Prediction.Shared.Dto;

/// <summary>
/// Data transfer object representing the current status of the API.
/// Used to provide information about the API's operational status and version.
/// </summary>
/// <param name="Status">The current operational status of the API.</param>
/// <param name="Version">The version number of the API.</param>
/// <param name="Timestamp">The timestamp when the status was checked.</param>
public record ApiStatusDto(
    string Status,
    string Version,
    DateTime Timestamp
);