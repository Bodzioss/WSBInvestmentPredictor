using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WSBInvestmentPredictor.Prediction.Models;

namespace WSBInvestmentPredictor.Prediction.Services;

public class PredictionService
{
    private readonly HttpClient _http;

    public PredictionService(HttpClient http) => _http = http;

    public async Task<PredictionResultDto?> PredictAsync(MarketDataInput input)
    {
        var response = await _http.PostAsJsonAsync("api/prediction/predict", input);
        return await response.Content.ReadFromJsonAsync<PredictionResultDto>();
    }
}