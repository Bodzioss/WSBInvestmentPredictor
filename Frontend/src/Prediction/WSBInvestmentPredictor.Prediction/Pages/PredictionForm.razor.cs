using Microsoft.AspNetCore.Components;
using Radzen;
using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WSBInvestmentPredictor.Prediction.Models;
using WSBInvestmentPredictor.Frontend.Shared.Services;

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
