using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace WSBInvestmentPredictor.Technology.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            _ => new
            {
                error = "Wystąpił nieoczekiwany błąd serwera.",
                status = StatusCodes.Status500InternalServerError
            }
        };

        context.Response.StatusCode = response.status;
        var json = JsonSerializer.Serialize(new { response.error });

        return context.Response.WriteAsync(json);
    }
}