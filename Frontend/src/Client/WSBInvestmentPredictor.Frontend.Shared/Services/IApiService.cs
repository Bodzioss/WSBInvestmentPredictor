using System.Threading.Tasks;

namespace WSBInvestmentPredictor.Frontend.Shared.Services;

/// <summary>
/// Interface for handling HTTP API communication.
/// Defines methods for making GET and POST requests with error handling.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Makes a POST request to the specified URL with the given payload.
    /// </summary>
    /// <typeparam name="T">The type of response data expected</typeparam>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="payload">The data to send in the request body</param>
    /// <param name="errorContext">Optional context for error messages</param>
    /// <returns>The deserialized response data, or default if the request fails</returns>
    Task<T?> PostAsync<T>(string url, object payload, string? errorContext = null);

    /// <summary>
    /// Makes a GET request to the specified URL.
    /// </summary>
    /// <typeparam name="T">The type of response data expected</typeparam>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="errorContext">Optional context for error messages</param>
    /// <returns>The deserialized response data, or default if the request fails</returns>
    Task<T?> GetAsync<T>(string url, string? errorContext = null);
}