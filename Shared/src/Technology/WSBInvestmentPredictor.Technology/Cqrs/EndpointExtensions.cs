using System.Reflection;
using System.Text.RegularExpressions;

namespace WSBInvestmentPredictor.Technology.Cqrs;

/// <summary>
/// Provides extension methods for handling API endpoint parameters in CQRS requests.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Replaces parameter placeholders in the endpoint string with values from the request object.
    /// </summary>
    /// <param name="endpoint">The endpoint string containing parameter placeholders in the format {parameterName}.</param>
    /// <param name="request">The request object containing the parameter values.</param>
    /// <returns>The endpoint string with parameter placeholders replaced by their corresponding values.</returns>
    /// <remarks>
    /// Parameter names in the endpoint string are matched case-insensitively against property names in the request object.
    /// Values are URL-encoded before being inserted into the endpoint string.
    /// </remarks>
    public static string ApplyParams(this string endpoint, object request)
    {
        var matches = Regex.Matches(
            endpoint,
            @"{([^}]+)}",
            RegexOptions.None,
            TimeSpan.FromMilliseconds(200));

        foreach (Match match in matches)
        {
            var paramName = match.Groups[1].Value;
            var prop = request.GetType().GetProperty(
                paramName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop == null)
                continue;

            var value = prop.GetValue(request)?.ToString();
            endpoint = endpoint.Replace(match.Value, Uri.EscapeDataString(value ?? string.Empty));
        }

        return endpoint;
    }
}