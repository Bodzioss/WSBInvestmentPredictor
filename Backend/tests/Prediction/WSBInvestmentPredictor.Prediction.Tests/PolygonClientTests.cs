using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using WSBInvestmentPredictor.Prediction.Infrastructure.MarketData.Dto;
using WSBInvestmentPredictor.Prediction.MarketData;
using Xunit;

namespace WSBInvestmentPredictor.Prediction.UnitTests;

public class PolygonClientTests
{
    [Fact]
    public async Task GetDailyOhlcvAsync_ReturnsData_WhenResponseIsValid()
    {
        // Arrange
        var fakeResponse = new PolygonCandleResponseDto
        {
            ticker = "AAPL",
            status = "OK",
            results =
        [
            new PolygonBar
            {
                t = 1672531200000, // timestamp ms
                o = 100f,
                h = 110f,
                l = 90f,
                c = 105f,
                v = 100000f
            }
        ]
        };

        var httpHandlerMock = new Mock<HttpMessageHandler>();
        httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(fakeResponse))
            });

        var httpClient = new HttpClient(httpHandlerMock.Object);
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Polygon:ApiKey"]).Returns("test_api_key");

        var polygonClient = new PolygonClient(httpClient, configMock.Object);

        // Act
        var result = await polygonClient.GetDailyOhlcvAsync("AAPL", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal("2023-01-01", result[0].Date); // Twój klient mapuje t na Date yyyy-MM-dd
        Assert.Equal(100f, result[0].Open);
    }

    [Fact]
    public async Task GetDailyOhlcvAsync_ThrowsException_WhenNoDataReturned()
    {
        // Arrange
        var emptyResponse = new PolygonCandleResponseDto
        {
            ticker = "AAPL",
            status = "OK",
            results = new List<PolygonBar>() // pusta lista
        };

        var httpHandlerMock = new Mock<HttpMessageHandler>();
        httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(emptyResponse))
            });

        var httpClient = new HttpClient(httpHandlerMock.Object);
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Polygon:ApiKey"]).Returns("test_api_key");

        var polygonClient = new PolygonClient(httpClient, configMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            polygonClient.GetDailyOhlcvAsync("AAPL", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow));
    }
}