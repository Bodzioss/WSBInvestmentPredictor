using System.ComponentModel.DataAnnotations;

namespace WSBInvestmentPredictor.Prediction.Models;

/// <summary>
/// Represents raw market data input from the user, used for Prediction.
/// This matches the minimal fields required by the backend API.
/// </summary>
public class MarketDataInput
{
    [Required]
    public string Date { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public float Open { get; set; }

    [Range(0, double.MaxValue)]
    public float High { get; set; }

    [Range(0, double.MaxValue)]
    public float Low { get; set; }

    [Range(0, double.MaxValue)]
    public float Close { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "Volume must be greater than 0")]
    public float Volume { get; set; }
}