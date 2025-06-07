using Microsoft.AspNetCore.Builder;

namespace WSBInvestmentPredictor.Technology.Middleware;

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}