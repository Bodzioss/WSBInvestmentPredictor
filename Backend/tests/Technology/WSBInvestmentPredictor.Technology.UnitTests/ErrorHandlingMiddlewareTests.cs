using Microsoft.AspNetCore.Http;
using System.Text.Json;
using WSBInvestmentPredictor.Technology.Middleware;
using Xunit;

namespace WSBInvestmentPredictor.Technology.UnitTests;

public class ErrorHandlingMiddlewareTests
{
    [Fact]
    public async Task Returns_500_For_Unexpected_Exception()
    {
        // Arrange
        var middleware = new ErrorHandlingMiddleware(context =>
            throw new Exception("Something went wrong"));

        var context = new DefaultHttpContext();
        var stream = new MemoryStream();
        context.Response.Body = stream;

        // Act
        await middleware.Invoke(context);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(stream).ReadToEndAsync();

        var json = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("Wystąpił nieoczekiwany błąd serwera.", json["error"].GetString());
        Assert.Equal(500, json["status"].GetInt32());
    }
}