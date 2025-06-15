using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.API.Cqrs;

public static class CqrsEndpointMapper
{
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

            var resultType = requestType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                ?.GetGenericArguments().FirstOrDefault();

            if (resultType is null)
                continue;

            var routeBuilder = httpMethod switch
            {
                "GET" => app.MapGet(endpoint, CreateHandler(requestType, resultType)),
                "POST" => app.MapPost(endpoint, CreateHandler(requestType, resultType)),
                "PUT" => app.MapPut(endpoint, CreateHandler(requestType, resultType)),
                "DELETE" => app.MapDelete(endpoint, CreateHandler(requestType, resultType)),
                _ => throw new NotSupportedException($"HTTP method {httpMethod} is not supported")
            };

            routeBuilder
                .WithName(requestType.Name)
                .RequireCors(corsPolicyName);
        }
    }

    private static Delegate CreateHandler(Type requestType, Type resultType)
    {
        var method = typeof(CqrsEndpointMapper)
            .GetMethod(nameof(HandleAsync), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(requestType, resultType);

        var handlerType = typeof(Func<,,>)
            .MakeGenericType(requestType, typeof(IMediator), typeof(Task<>).MakeGenericType(resultType));

        var invoke = method.CreateDelegate(handlerType);

        // ręczna lambda z [FromBody] i [FromServices]
        var lambdaMethod = typeof(CqrsEndpointMapper)
            .GetMethod(nameof(WrapHandler), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(requestType, resultType);

        return lambdaMethod.Invoke(null, new object[] { invoke }) as Delegate;
    }

    private static async Task<TResult> HandleAsync<TRequest, TResult>(TRequest request, IMediator mediator)
        where TRequest : IRequest<TResult>
    {
        return await mediator.Send(request);
    }

    private static Delegate WrapHandler<TRequest, TResult>(Func<TRequest, IMediator, Task<TResult>> handler)
        where TRequest : IRequest<TResult>
    {
        var props = typeof(TRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // GET z pustym requestem
        if (props.Length == 0)
        {
            return async ([FromServices] IMediator mediator) =>
            {
                var request = Activator.CreateInstance<TRequest>(); // zamiast new TRequest()
                return await handler(request, mediator);
            };
        }

        // GET z parametrami (z query string)
        if (HttpMethods.IsGet(typeof(TRequest)
            .GetCustomAttribute<ApiRequestAttribute>()?.HttpMethod))
        {
            return async ([FromQuery] TRequest request, [FromServices] IMediator mediator) =>
            {
                return await handler(request, mediator);
            };
        }

        // domyślnie FromBody (POST, PUT, DELETE)
        return async ([FromBody] TRequest request, [FromServices] IMediator mediator) =>
        {
            return await handler(request, mediator);
        };
    }
} 