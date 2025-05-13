using System.Reflection;
using System.Text.RegularExpressions;

namespace WSBInvestmentPredictor.Technology.Cqrs;

public static class EndpointExtensions
{
    public static string ApplyParams(this string endpoint, object request)
    {
        var matches = Regex.Matches(endpoint, @"{([^}]+)}");

        foreach (Match match in matches)
        {
            var paramName = match.Groups[1].Value;
            var prop = request.GetType().GetProperty(paramName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop is null)
                throw new InvalidOperationException($"Brak właściwości '{paramName}' w {request.GetType().Name}.");

            var value = prop.GetValue(request)?.ToString() ?? "";
            endpoint = endpoint.Replace($"{{{paramName}}}", Uri.EscapeDataString(value));
        }

        return endpoint;
    }
}
