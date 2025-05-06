using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Radzen;

namespace WSBInvestmentPredictor.Frontend.Shared.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _http;
    private readonly NotificationService _notification;

    public ApiService(HttpClient http, NotificationService notification)
    {
        _http = http;
        _notification = notification;
    }

    public async Task<T?> PostAsync<T>(string url, object payload, string? errorContext = null)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            var errorText = await response.Content.ReadAsStringAsync();
            NotifyError($"API error {response.StatusCode}", errorText, errorContext);
        }
        catch (Exception ex)
        {
            NotifyError("Unexpected error", ex.Message, errorContext);
        }

        return default;
    }

    public async Task<T?> GetAsync<T>(string url, string? errorContext = null)
    {
        try
        {
            var response = await _http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            var errorText = await response.Content.ReadAsStringAsync();
            NotifyError($"API error {response.StatusCode}", errorText, errorContext);
        }
        catch (Exception ex)
        {
            NotifyError("Unexpected error", ex.Message, errorContext);
        }

        return default;
    }

    private void NotifyError(string title, string detail, string? context = null)
    {
        _notification.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Error,
            Summary = context != null ? $"{context} - {title}" : title,
            Detail = detail,
            Duration = 6000,
            Style = "position: fixed; top: 20px; right: 20px; z-index: 9999;"
        });
    }
}
