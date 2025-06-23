using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using WSBInvestmentPredictor.Frontend.Shared.Services;
using WSBInvestmentPredictor.Prediction.Models;

namespace WSBInvestmentPredictor.Prediction.Pages;

/// <summary>
/// Component for making predictions based on raw market data input.
/// Allows users to input market data manually and get predictions for future returns.
/// </summary>
public partial class PredictionForm : ComponentBase
{
    [Inject] protected ApiService Api { get; set; } = default!;

    // Input model for market data
    protected MarketDataInput input = new();

    // Stores the prediction result
    protected float? predictionResult;

    /// <summary>
    /// Handles the form submission and makes a prediction request to the API.
    /// </summary>
    protected async Task HandleValidSubmit()
    {
        var payload = new List<MarketDataInput> { input };

        var dto = await Api.PostAsync<PredictionResultDto>(
            "/api/Prediction/predict-from-raw",
            payload,
            "Prediction"
        );

        predictionResult = dto?.Prediction;
    }
}