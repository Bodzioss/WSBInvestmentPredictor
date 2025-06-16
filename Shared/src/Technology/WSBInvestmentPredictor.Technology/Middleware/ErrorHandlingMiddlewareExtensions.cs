using Microsoft.AspNetCore.Builder;

namespace WSBInvestmentPredictor.Technology.Middleware;

/// <summary>
/// Provides extension methods for adding error handling middleware to the application pipeline.
/// </summary>
public static class ErrorHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adds the error handling middleware to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <returns>The application builder instance for method chaining.</returns>
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}