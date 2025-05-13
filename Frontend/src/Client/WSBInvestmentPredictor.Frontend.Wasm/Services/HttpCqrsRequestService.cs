using System.Net.Http.Json;
using System.Reflection;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Frontend.Wasm.Services.Cqrs;

public class HttpCqrsRequestService : ICqrsRequestService
{
    private readonly HttpClient _http;

    public HttpCqrsRequestService(HttpClient http)
    {
        _http = http;
    }

    public async Task<TResult> Handle<TRequest, TResult>(TRequest request)
        where TRequest : class
    {
        var attr = typeof(TRequest).GetCustomAttribute<ApiRequestAttribute>()
                   ?? throw new InvalidOperationException($"Brakuje ApiRequestAttribute w {typeof(TRequest).Name}.");

        var endpoint = attr.Endpoint.ApplyParams(request);

        return attr.HttpMethod switch
        {
            "GET" => await _http.GetFromJsonAsync<TResult>(endpoint)
                       ?? throw new InvalidOperationException("Brak danych z API."),
            "POST" => await PostJson<TRequest, TResult>(endpoint, request),
            _ => throw new NotImplementedException($"Nieobsługiwana metoda HTTP: {attr.HttpMethod}")
        };
    }

    public async Task Handle<TRequest>(TRequest request)
        where TRequest : class
    {
        var attr = typeof(TRequest).GetCustomAttribute<ApiRequestAttribute>()
                   ?? throw new InvalidOperationException($"Brakuje ApiRequestAttribute w {typeof(TRequest).Name}.");

        var endpoint = attr.Endpoint.ApplyParams(request);

        var response = attr.HttpMethod switch
        {
            "POST" => await _http.PostAsJsonAsync(endpoint, request),
            "PUT" => await _http.PutAsJsonAsync(endpoint, request),
            "DELETE" => await _http.DeleteAsync(endpoint),
            _ => throw new NotImplementedException()
        };

        response.EnsureSuccessStatusCode();
    }

    private async Task<TResult> PostJson<TRequest, TResult>(string url, TRequest request)
    {
        var response = await _http.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TResult>();
        return result ?? throw new InvalidOperationException("Brak danych w odpowiedzi POST.");
    }
}
