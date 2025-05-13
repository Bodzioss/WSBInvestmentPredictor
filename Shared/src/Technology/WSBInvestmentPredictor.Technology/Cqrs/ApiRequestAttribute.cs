namespace WSBInvestmentPredictor.Technology.Cqrs;

[AttributeUsage(AttributeTargets.Class)]
public class ApiRequestAttribute : Attribute
{
    public string Endpoint { get; }
    public string HttpMethod { get; }

    public ApiRequestAttribute(string endpoint, string httpMethod = "POST")
    {
        Endpoint = endpoint;
        HttpMethod = httpMethod.ToUpper();
    }
}