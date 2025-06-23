using Radzen;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WSBInvestmentPredictor.Frontend.Shared.Services;

/// <summary>
/// Service for handling HTTP API communication.
/// Provides methods for making GET and POST requests with error handling and notifications.
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _http;
    private readonly NotificationService _notification;

    /// <summary>
    /// Initializes a new instance of the ApiService class.
    /// </summary>
    /// <param name="http">The HTTP client for making requests</param>
    /// <param name="notification">The notification service for displaying errors</param>
    public ApiService(HttpClient http, NotificationService notification)
    {
        _http = http;
        _notification = notification;
    }

    /// <summary>
    /// Makes a POST request to the specified URL with the given payload.
    /// </summary>
    /// <typeparam name="T">The type of response data expected</typeparam>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="payload">The data to send in the request body</param>
    /// <param name="errorContext">Optional context for error messages</param>
    /// <returns>The deserialized response data, or default if the request fails</returns>
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

    /// <summary>
    /// Makes a GET request to the specified URL.
    /// </summary>
    /// <typeparam name="T">The type of response data expected</typeparam>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="errorContext">Optional context for error messages</param>
    /// <returns>The deserialized response data, or default if the request fails</returns>
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

    /// <summary>
    /// Displays an error notification to the user.
    /// </summary>
    /// <param name="title">The error title</param>
    /// <param name="detail">The error details</param>
    /// <param name="context">Optional context to prefix the error title</param>
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