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

        var (statusCode, message) = exception switch
        {
            ArgumentException argEx => (StatusCodes.Status400BadRequest, argEx.Message),
            InvalidOperationException invOpEx => (StatusCodes.Status400BadRequest, invOpEx.Message),
            _ => (StatusCodes.Status500InternalServerError, "Wystąpił nieoczekiwany błąd serwera.")
        };

        context.Response.StatusCode = statusCode;

        var errorPayload = JsonSerializer.Serialize(new
        {
            error = message,
            status = statusCode
        });

        return context.Response.WriteAsync(errorPayload);
    }
}
