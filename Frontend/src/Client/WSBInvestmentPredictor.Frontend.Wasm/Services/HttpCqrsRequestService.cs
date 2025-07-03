using System.Net.Http.Json;
using System.Reflection;
using WSBInvestmentPredictor.Technology.Cqrs;
using System.Text.RegularExpressions;

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
                   ?? throw new InvalidOperationException($"Missing ApiRequestAttribute in {typeof(TRequest).Name}.");

        var endpoint = ApplyParams(attr.Endpoint, request);
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

                case "PUT":
                    response = await _http.PutAsJsonAsync(endpoint, request);
                    break;

                case "DELETE":
                    var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, endpoint)
                    {
                        Content = JsonContent.Create(request)
                    };
                    response = await _http.SendAsync(deleteRequest);
                    break;

                default:
                    throw new NotImplementedException($"Unsupported HTTP method: {attr.HttpMethod}");
            }

            try
            {
                response.EnsureSuccessStatusCode();
                if (response.Content.Headers.ContentLength == 0 || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return default!;
                var result = await response.Content.ReadFromJsonAsync<TResult>();
                return result ?? throw new InvalidOperationException("No data in response.");
            }
            catch (HttpRequestException ex)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response from {endpoint}: {content}");
                throw new HttpRequestException($"Error executing {attr.HttpMethod} request to {endpoint}: {content}", ex);
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
                   ?? throw new InvalidOperationException($"Missing ApiRequestAttribute in {typeof(TRequest).Name}.");

        var endpoint = ApplyParams(attr.Endpoint, request);
        Console.WriteLine($"Sending {attr.HttpMethod} request to: {endpoint}");

        try
        {
            var response = attr.HttpMethod switch
            {
                "POST" => await _http.PostAsJsonAsync(endpoint, request),
                "PUT" => await _http.PutAsJsonAsync(endpoint, request),
                "DELETE" => await SendDeleteWithBody(endpoint, request),
                _ => throw new NotImplementedException($"Unsupported HTTP method: {attr.HttpMethod}")
            };

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response from {endpoint}: {content}");
                throw new HttpRequestException($"Error executing {attr.HttpMethod} request to {endpoint}: {content}", ex);
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
            return result ?? throw new InvalidOperationException("No data in POST response.");
        }
        catch (HttpRequestException ex)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error response from {url}: {content}");
            throw new HttpRequestException($"Error executing POST request to {url}: {content}", ex);
        }
    }

    private static string ApplyParams(string endpoint, object request)
    {
        Console.WriteLine($"ApplyParams: endpoint before: {endpoint}");
        Console.WriteLine($"ApplyParams: request type: {request.GetType().Name}");
        var matches = Regex.Matches(endpoint, "{([^}]+)}");
        foreach (var match in matches.Cast<Match>())
        {
            var paramName = match.Groups[1].Value;
            var requestType = request.GetType();
            var prop = requestType.GetProperties().SingleOrDefault(x => x.Name.Equals(paramName, StringComparison.CurrentCultureIgnoreCase));
            if (prop == null)
                throw new InvalidOperationException($"Missing property {paramName} in {requestType.Name}");
            var value = prop.GetValue(request)?.ToString();
            Console.WriteLine($"ApplyParams: substituting {{{paramName}}} -> {value}");
            endpoint = endpoint.Replace($"{{{paramName}}}", value);
        }
        Console.WriteLine($"ApplyParams: endpoint after: {endpoint}");
        return endpoint;
    }

    private async Task<HttpResponseMessage> SendDeleteWithBody(string endpoint, object request)
    {
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, endpoint)
        {
            Content = JsonContent.Create(request)
        };
        return await _http.SendAsync(deleteRequest);
    }
}