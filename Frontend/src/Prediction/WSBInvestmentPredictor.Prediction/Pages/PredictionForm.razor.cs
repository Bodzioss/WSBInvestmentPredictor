using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using WSBInvestmentPredictor.Frontend.Shared.Services;
using WSBInvestmentPredictor.Prediction.Models;

namespace WSBInvestmentPredictor.Prediction.Pages;

public partial class PredictionForm : ComponentBase
{
    [Inject] protected ApiService Api { get; set; } = default!;

    protected MarketDataInput input = new();
    protected float? predictionResult;

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