namespace WSBInvestmentPredictor.Prediction.Shared.Dto;

/// <summary>
/// Represents a company's stock ticker symbol and name.
/// Used for identifying and displaying company information in the application.
/// </summary>
/// <param name="Ticker">The stock ticker symbol of the company.</param>
/// <param name="Name">The full name of the company.</param>
public record CompanyTicker(string Ticker, string Name);