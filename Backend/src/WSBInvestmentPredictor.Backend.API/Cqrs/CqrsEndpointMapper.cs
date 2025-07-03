using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.RegularExpressions;
using WSBInvestmentPredictor.Technology.Cqrs;
using System.Linq.Expressions;

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

            var isCommand = requestType.GetInterfaces().Any(i => i == typeof(IRequest));
            var isQuery = requestType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            if (!isCommand && !isQuery)
                continue;

            Type? resultType = null;
            if (isQuery)
            {
                resultType = requestType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                    .GetGenericArguments().First();
            }

            var routeDelegate = CreateUniversalHandler(requestType, resultType, isQuery, endpoint);
            var routeBuilder = app.MapMethods(endpoint, new[] { httpMethod }, routeDelegate);

            routeBuilder
                .WithName(requestType.Name)
                .RequireCors(corsPolicyName);
        }
    }

    private static Delegate CreateUniversalHandler(Type requestType, Type? resultType, bool isQuery, string endpoint)
    {
        var props = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var routeParams = Regex.Matches(endpoint, "{(.*?)}")
                               .Cast<Match>()
                               .Select(m => m.Groups[1].Value)
                               .ToList();

        // Handler without parameters
        if (props.Length == 0)
        {
            if (isQuery)
            {
                return async ([FromServices] IMediator mediator) =>
                {
                    var request = Activator.CreateInstance(requestType)!;
                    return await mediator.Send(request);
                };
            }
            else
            {
                return async ([FromServices] IMediator mediator) =>
                {
                    var request = Activator.CreateInstance(requestType)!;
                    await mediator.Send(request);
                    return Results.Ok();
                };
            }
        }

        // Handler with path parameters
        var pathParams = props
            .Where(p => p.GetCustomAttribute<FromRouteAttribute>() != null)
            .Select(p => p.Name)
            .ToList();

        if (pathParams.Any())
        {
            var pathParamString = string.Join("/", pathParams.Select(p => $"{{{p}}}"));
            return DynamicHandlerFactory.Create(requestType, resultType, isQuery, pathParams);
        }

        // GET with query string
        var isGet = HttpMethods.IsGet(requestType.GetCustomAttribute<ApiRequestAttribute>()?.HttpMethod);
        if (isGet)
        {
            if (isQuery)
            {
                return async ([FromQuery] object request, [FromServices] IMediator mediator) =>
                {
                    var json = JsonSerializer.Serialize(request, _jsonOptions);
                    var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
                    return await mediator.Send(typedRequest);
                };
            }
            else
            {
                return async ([FromQuery] object request, [FromServices] IMediator mediator) =>
                {
                    var json = JsonSerializer.Serialize(request, _jsonOptions);
                    var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
                    await mediator.Send(typedRequest);
                    return Results.Ok();
                };
            }
        }

        // DELETE, POST, PUT with body
        var httpMethod = requestType.GetCustomAttribute<ApiRequestAttribute>()?.HttpMethod?.ToUpperInvariant();
        var isBodyMethod = httpMethod == "DELETE" || httpMethod == "POST" || httpMethod == "PUT";
        if (isBodyMethod)
        {
            if (isQuery)
            {
                return async ([FromBody] object request, [FromServices] IMediator mediator) =>
                {
                    var json = JsonSerializer.Serialize(request, _jsonOptions);
                    var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
                    return await mediator.Send(typedRequest);
                };
            }
            else
            {
                return async ([FromBody] object request, [FromServices] IMediator mediator) =>
                {
                    var json = JsonSerializer.Serialize(request, _jsonOptions);
                    var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
                    await mediator.Send(typedRequest);
                    return Results.Ok();
                };
            }
        }

        // Default: body
        if (isQuery)
        {
            return async ([FromBody] object request, [FromServices] IMediator mediator) =>
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
                return await mediator.Send(typedRequest);
            };
        }
        else
        {
            return async ([FromBody] object request, [FromServices] IMediator mediator) =>
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var typedRequest = JsonSerializer.Deserialize(json, requestType, _jsonOptions)!;
                await mediator.Send(typedRequest);
                return Results.Ok();
            };
        }
    }
}

public static class DynamicHandlerFactory
{
    public static Delegate Create(Type requestType, Type? resultType, bool isQuery, List<string> routeParams)
    {
        // Tworzymy parametry metody
        var parameterTypes = routeParams.Select(_ => typeof(string)).ToList();
        parameterTypes.Add(typeof(IMediator));

        // Tworzymy dynamiczny delegat
        if (isQuery)
        {
            return CreateQueryDelegate(requestType, resultType, routeParams);
        }
        else
        {
            return CreateCommandDelegate(requestType, routeParams);
        }
    }

    private static Delegate CreateCommandDelegate(Type requestType, List<string> routeParams)
    {
        var parameterTypes = routeParams.Select(_ => typeof(string)).Append(typeof(IMediator)).ToArray();

        return parameterTypes.Length switch
        {
            2 => (Func<string, IMediator, Task<IResult>>)(async (p1, mediator) =>
            {
                var request = Activator.CreateInstance(requestType, p1)!;
                await mediator.Send((dynamic)request);
                return Results.Ok();
            }),
            3 => (Func<string, string, IMediator, Task<IResult>>)(async (p1, p2, mediator) =>
            {
                var request = Activator.CreateInstance(requestType, p1, p2)!;
                await mediator.Send((dynamic)request);
                return Results.Ok();
            }),
            _ => throw new NotSupportedException("Too many route parameters for auto-mapping.")
        };
    }

    private static Delegate CreateQueryDelegate(Type requestType, Type? resultType, List<string> routeParams)
    {
        var parameterTypes = routeParams.Select(_ => typeof(string)).Append(typeof(IMediator)).ToArray();

        return parameterTypes.Length switch
        {
            2 => (Func<string, IMediator, Task<object>>)(async (p1, mediator) =>
            {
                var request = Activator.CreateInstance(requestType, p1)!;
                return await mediator.Send((dynamic)request);
            }),
            3 => (Func<string, string, IMediator, Task<object>>)(async (p1, p2, mediator) =>
            {
                var request = Activator.CreateInstance(requestType, p1, p2)!;
                return await mediator.Send((dynamic)request);
            }),
            _ => throw new NotSupportedException("Too many route parameters for auto-mapping.")
        };
    }
}