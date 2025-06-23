using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using WSBInvestmentPredictor.Prediction.Infrastructure.MarketData.Dto;
using WSBInvestmentPredictor.Prediction.Shared.Dto;

namespace WSBInvestmentPredictor.Prediction.MarketData;

public class PolygonClient : IPolygonClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public PolygonClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        var apiKey = config["Polygon:ApiKey"];
        Console.WriteLine($"PolygonClient: API Key from config: {apiKey}");
        _apiKey = apiKey ?? throw new ArgumentNullException("Polygon API key is missing.");
    }

    public async Task<List<RawMarketData>> GetDailyOhlcvAsync(string symbol, DateTime from, DateTime to)
    {
        var fromStr = from.ToString("yyyy-MM-dd");
        var toStr = to.ToString("yyyy-MM-dd");

        var url = $"https://api.polygon.io/v2/aggs/ticker/{symbol.ToUpper()}/range/1/day/{fromStr}/{toStr}?adjusted=true&sort=asc&apiKey={_apiKey}";
        Console.WriteLine($"PolygonClient: Making request to: {url}");

        var response = await _httpClient.GetFromJsonAsync<PolygonCandleResponseDto>(url);

        if (response == null || response.results == null || response.results.Count == 0)
            throw new InvalidOperationException("No data returned from Polygon.");

        var result = response.results.Select(r => new RawMarketData(
            Date: DateTimeOffset.FromUnixTimeMilliseconds(r.t).Date.ToString("yyyy-MM-dd"),
            Open: r.o,
            High: r.h,
            Low: r.l,
            Close: r.c,
            Volume: r.v
        )).ToList();

        return result;
    }
}