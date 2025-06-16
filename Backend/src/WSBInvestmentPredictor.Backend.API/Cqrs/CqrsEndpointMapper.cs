using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Backend.API.Cqrs;

/// <summary>
/// Provides functionality to automatically map CQRS requests to HTTP endpoints.
/// This class handles the registration of command and query handlers as API endpoints
/// based on the ApiRequestAttribute decoration.
/// </summary>
public static class CqrsEndpointMapper
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Maps all CQRS requests decorated with ApiRequestAttribute to their corresponding HTTP endpoints.
    /// </summary>
    /// <param name="app">The WebApplication instance to configure endpoints for.</param>
    /// <param name="assembly">The assembly containing the CQRS request types.</param>
    /// <param name="corsPolicyName">The name of the CORS policy to apply to the endpoints.</param>
    public static void MapCqrsEndpoints(this WebApplication app, Assembly assembly, string corsPolicyName)
    {
        var requestTypes = assembly
            .GetTypes()
            .Where(t => t.GetCustomAttribute<ApiRequestAttribute>() is not null);

        foreach (var requestType in requestTypes)
        {
            var attribute = requestType.GetCustomAttribute<ApiRequestAttribute>()!;
            var httpMethod = attribute.HttpMethod.ToUpperInvariant();
            var endpoint = attribute.Endpoint;

            // Sprawdź, czy to komenda czy zapytanie
            var isCommand = requestType.GetInterfaces().Any(i => i == typeof(IRequest));
            var isQuery = requestType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            if (!isCommand && !isQuery)
                continue;

            if (isQuery)
            {
                var resultType = requestType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                    .GetGenericArguments().First();

                var routeBuilder = httpMethod switch
                {
                    "GET" => app.MapGet(endpoint, CreateQueryHandler(requestType, resultType)),
                    "POST" => app.MapPost(endpoint, CreateQueryHandler(requestType, resultType)),
                    "PUT" => app.MapPut(endpoint, CreateQueryHandler(requestType, resultType)),
                    "DELETE" => app.MapDelete(endpoint, CreateQueryHandler(requestType, resultType)),
                    _ => throw new NotSupportedException($"HTTP method {httpMethod} is not supported")
                };

                routeBuilder
                    .WithName(requestType.Name)
                    .RequireCors(corsPolicyName);
            }
            else
            {
                var routeBuilder = httpMethod switch
                {
                    "GET" => app.MapGet(endpoint, CreateCommandHandler(requestType)),
                    "POST" => app.MapPost(endpoint, CreateCommandHandler(requestType)),
                    "PUT" => app.MapPut(endpoint, CreateCommandHandler(requestType)),
                    "DELETE" => app.MapDelete(endpoint, CreateCommandHandler(requestType)),
                    _ => throw new NotSupportedException($"HTTP method {httpMethod} is not supported")
                };

                routeBuilder
                    .WithName(requestType.Name)
                    .RequireCors(corsPolicyName);
            }
        }
    }

    /// <summary>
    /// Creates a delegate that handles query requests and returns a result.
    /// </summary>
    /// <param name="requestType">The type of the query request.</param>
    /// <param name="resultType">The type of the query result.</param>
    /// <returns>A delegate that processes the query request and returns the result.</returns>
    private static Delegate CreateQueryHandler(Type requestType, Type resultType)
    {
        var props = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var isGet = HttpMethods.IsGet(requestType.GetCustomAttribute<ApiRequestAttribute>()?.HttpMethod);

        if (props.Length == 0)
        {
            return async ([FromServices] IMediator mediator) =>
            {
                var request = Activator.CreateInstance(requestType)!;
                return await mediator.Send(request);
            };
        }

        if (isGet)
        {
            return async ([FromQuery] object request, [FromServices] IMediator mediator) =>
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
                return await mediator.Send(typedRequest);
            };
        }

        return async ([FromBody] object request, [FromServices] IMediator mediator) =>
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
            return await mediator.Send(typedRequest);
        };
    }

    /// <summary>
    /// Creates a delegate that handles command requests without returning a result.
    /// </summary>
    /// <param name="requestType">The type of the command request.</param>
    /// <returns>A delegate that processes the command request and returns an OK result.</returns>
    private static Delegate CreateCommandHandler(Type requestType)
    {
        var props = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var isGet = HttpMethods.IsGet(requestType.GetCustomAttribute<ApiRequestAttribute>()?.HttpMethod);

        if (props.Length == 0)
        {
            return async ([FromServices] IMediator mediator) =>
            {
                var request = Activator.CreateInstance(requestType)!;
                await mediator.Send(request);
                return Results.Ok();
            };
        }

        if (isGet)
        {
            return async ([FromQuery] object request, [FromServices] IMediator mediator) =>
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
                await mediator.Send(typedRequest);
                return Results.Ok();
            };
        }

        return async ([FromBody] object request, [FromServices] IMediator mediator) =>
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
            await mediator.Send(typedRequest);
            return Results.Ok();
        };
    }
}