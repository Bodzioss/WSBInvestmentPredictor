namespace WSBInvestmentPredictor.Technology.Cqrs;

/// <summary>
/// Attribute used to mark CQRS request classes with their corresponding API endpoint information.
/// This attribute is used to specify the HTTP endpoint and method for handling the request.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ApiRequestAttribute : Attribute
{
    /// <summary>
    /// Gets the API endpoint path for the request.
    /// </summary>
    public string Endpoint { get; }

    /// <summary>
    /// Gets the HTTP method to be used for the request.
    /// </summary>
    public string HttpMethod { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiRequestAttribute"/> class.
    /// </summary>
    /// <param name="endpoint">The API endpoint path for the request.</param>
    /// <param name="httpMethod">The HTTP method to be used (defaults to "POST").</param>
    public ApiRequestAttribute(string endpoint, string httpMethod = "POST")
    {
        Endpoint = endpoint;
        HttpMethod = httpMethod.ToUpperInvariant();
    }
}