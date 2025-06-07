using Xunit;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Technology.UnitTests;

public class EndpointExtensionsTests
{
    private class DummyRequest
    {
        public string Id { get; set; } = "123";
        public string Name { get; set; } = "test name";
        public string NullProp { get; set; } = null;
        public int Number { get; set; } = 42;
    }

    [Fact]
    public void ApplyParams_ReplacesPlaceholdersWithValues()
    {
        // Arrange
        var endpoint = "/api/items/{Id}/details/{Name}";
        var request = new DummyRequest();

        // Act
        var result = endpoint.ApplyParams(request);

        // Assert
        Assert.Equal("/api/items/123/details/test%20name", result);
    }

    [Fact]
    public void ApplyParams_LeavesUnknownPlaceholdersIntact()
    {
        // Arrange
        var endpoint = "/api/items/{Unknown}/details/{Id}";
        var request = new DummyRequest();

        // Act
        var result = endpoint.ApplyParams(request);

        // Assert
        Assert.Equal("/api/items/{Unknown}/details/123", result);
    }

    [Fact]
    public void ApplyParams_HandlesNullPropertyValue()
    {
        // Arrange
        var endpoint = "/api/items/{NullProp}/details";
        var request = new DummyRequest();

        // Act
        var result = endpoint.ApplyParams(request);

        // Assert
        Assert.Equal("/api/items//details", result);
    }

    [Fact]
    public void ApplyParams_EncodesSpecialCharacters()
    {
        // Arrange
        var endpoint = "/api/items/{Name}";
        var request = new DummyRequest();

        // Act
        var result = endpoint.ApplyParams(request);

        // Assert
        Assert.Equal("/api/items/test%20name", result);
    }

    [Fact]
    public void ApplyParams_ReplacesNumberProperty()
    {
        // Arrange
        var endpoint = "/api/items/{Number}";
        var request = new DummyRequest();

        // Act
        var result = endpoint.ApplyParams(request);

        // Assert
        Assert.Equal("/api/items/42", result);
    }

    [Fact]
    public void ApplyParams_EmptyEndpoint_ReturnsEmpty()
    {
        // Arrange
        var endpoint = "";
        var request = new DummyRequest();

        // Act
        var result = endpoint.ApplyParams(request);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ApplyParams_NoPlaceholders_ReturnsOriginal()
    {
        // Arrange
        var endpoint = "/api/static/path";
        var request = new DummyRequest();

        // Act
        var result = endpoint.ApplyParams(request);

        // Assert
        Assert.Equal("/api/static/path", result);
    }
}