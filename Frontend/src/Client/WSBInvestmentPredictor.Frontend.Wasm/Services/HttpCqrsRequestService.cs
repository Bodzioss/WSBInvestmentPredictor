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
        Console.WriteLine($"Sending {attr.HttpMethod} request to: {endpoint}");

        try
        {
            HttpResponseMessage response;
            switch (attr.HttpMethod)
            {
                case "GET":
                    response = await _http.GetAsync(endpoint);
                    break;

                case "POST":
                    response = await _http.PostAsJsonAsync(endpoint, request);
                    break;

                default:
                    throw new NotImplementedException($"Nieobsługiwana metoda HTTP: {attr.HttpMethod}");
            }

            try
            {
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<TResult>();
                return result ?? throw new InvalidOperationException("Brak danych w odpowiedzi.");
            }
            catch (HttpRequestException ex)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response from {endpoint}: {content}");
                throw new HttpRequestException($"Błąd podczas wykonywania żądania {attr.HttpMethod} {endpoint}: {content}", ex);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling {typeof(TRequest).Name}: {ex.Message}");
            throw;
        }
    }

    public async Task Handle<TRequest>(TRequest request)
        where TRequest : class
    {
        var attr = typeof(TRequest).GetCustomAttribute<ApiRequestAttribute>()
                   ?? throw new InvalidOperationException($"Brakuje ApiRequestAttribute w {typeof(TRequest).Name}.");

        var endpoint = attr.Endpoint.ApplyParams(request);
        Console.WriteLine($"Sending {attr.HttpMethod} request to: {endpoint}");

        try
        {
            var response = attr.HttpMethod switch
            {
                "POST" => await _http.PostAsJsonAsync(endpoint, request),
                "PUT" => await _http.PutAsJsonAsync(endpoint, request),
                "DELETE" => await _http.DeleteAsync(endpoint),
                _ => throw new NotImplementedException($"Nieobsługiwana metoda HTTP: {attr.HttpMethod}")
            };

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response from {endpoint}: {content}");
                throw new HttpRequestException($"Błąd podczas wykonywania żądania {attr.HttpMethod} {endpoint}: {content}", ex);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling {typeof(TRequest).Name}: {ex.Message}");
            throw;
        }
    }

    private async Task<TResult> PostJson<TRequest, TResult>(string url, TRequest request)
    {
        var response = await _http.PostAsJsonAsync(url, request);
        try
        {
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TResult>();
            return result ?? throw new InvalidOperationException("Brak danych w odpowiedzi POST.");
        }
        catch (HttpRequestException ex)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error response from {url}: {content}");
            throw new HttpRequestException($"Błąd podczas wykonywania żądania POST {url}: {content}", ex);
        }
    }
}