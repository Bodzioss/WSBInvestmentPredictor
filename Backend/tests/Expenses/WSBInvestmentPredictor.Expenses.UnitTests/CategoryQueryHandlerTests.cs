using MediatR;
using Moq;
using WSBInvestmentPredictor.Expenses.Application.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using Xunit;

namespace WSBInvestmentPredictor.Expenses.UnitTests;

public class CategoryQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CategoryQueryHandler _handler;

    public CategoryQueryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new CategoryQueryHandler(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_GetCategories_ShouldReturnOrderedCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 3, Name = "Transportation", Description = "Transport costs" },
            new Category { Id = 1, Name = "Groceries", Description = "Food shopping" },
            new Category { Id = 2, Name = "Entertainment", Description = "Movies and games" }
        };

        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

        var request = new GetCategories();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        
        // Verify categories are ordered by name
        Assert.Equal("Entertainment", result[0].Name);
        Assert.Equal("Groceries", result[1].Name);
        Assert.Equal("Transportation", result[2].Name);
        
        // Verify DTOs are correctly mapped
        Assert.Equal(2, result[0].Id);
        Assert.Equal("Entertainment", result[0].Name);
        Assert.Equal("Movies and games", result[0].Description);
        
        Assert.Equal(1, result[1].Id);
        Assert.Equal("Groceries", result[1].Name);
        Assert.Equal("Food shopping", result[1].Description);
        
        Assert.Equal(3, result[2].Id);
        Assert.Equal("Transportation", result[2].Name);
        Assert.Equal("Transport costs", result[2].Description);

        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_GetCategories_WithEmptyRepository_ShouldReturnEmptyList()
    {
        // Arrange
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());

        var request = new GetCategories();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_GetCategories_WithNullDescription_ShouldHandleCorrectly()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Test Category", Description = null }
        };

        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

        var request = new GetCategories();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Test Category", result[0].Name);
        Assert.Null(result[0].Description);
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_GetCategories_WithDuplicateNames_ShouldOrderCorrectly()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 2, Name = "Category B", Description = "Second" },
            new Category { Id = 1, Name = "Category A", Description = "First" },
            new Category { Id = 3, Name = "Category C", Description = "Third" }
        };

        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

        var request = new GetCategories();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        
        // Verify alphabetical ordering
        Assert.Equal("Category A", result[0].Name);
        Assert.Equal("Category B", result[1].Name);
        Assert.Equal("Category C", result[2].Name);
        
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
} 