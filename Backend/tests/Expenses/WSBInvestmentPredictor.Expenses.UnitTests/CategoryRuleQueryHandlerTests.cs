using MediatR;
using Moq;
using WSBInvestmentPredictor.Expenses.Application.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using Xunit;

namespace WSBInvestmentPredictor.Expenses.UnitTests;

public class CategoryRuleQueryHandlerTests
{
    private readonly Mock<ICategoryRuleRepository> _categoryRuleRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CategoryRuleQueryHandler _handler;

    public CategoryRuleQueryHandlerTests()
    {
        _categoryRuleRepositoryMock = new Mock<ICategoryRuleRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new CategoryRuleQueryHandler(_categoryRuleRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_GetCategoryRules_ShouldReturnRulesWithCategories()
    {
        // Arrange
        var rules = new List<CategoryRule>
        {
            new CategoryRule { Id = 1, Keyword = "grocery", CategoryId = 1 },
            new CategoryRule { Id = 2, Keyword = "gas", CategoryId = 2 },
            new CategoryRule { Id = 3, Keyword = "movie", CategoryId = 3 }
        };

        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Groceries", Description = "Food shopping" },
            new Category { Id = 2, Name = "Transportation", Description = "Gas and fuel" },
            new Category { Id = 3, Name = "Entertainment", Description = "Movies and games" }
        };

        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(rules);
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

        var request = new GetCategoryRules();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        // Verify first rule
        Assert.Equal(1, result[0].Id);
        Assert.Equal("grocery", result[0].Keyword);
        Assert.Equal(1, result[0].CategoryId);
        Assert.NotNull(result[0].Category);
        Assert.Equal(1, result[0].Category.Id);
        Assert.Equal("Groceries", result[0].Category.Name);
        Assert.Equal("Food shopping", result[0].Category.Description);

        // Verify second rule
        Assert.Equal(2, result[1].Id);
        Assert.Equal("gas", result[1].Keyword);
        Assert.Equal(2, result[1].CategoryId);
        Assert.NotNull(result[1].Category);
        Assert.Equal(2, result[1].Category.Id);
        Assert.Equal("Transportation", result[1].Category.Name);
        Assert.Equal("Gas and fuel", result[1].Category.Description);

        // Verify third rule
        Assert.Equal(3, result[2].Id);
        Assert.Equal("movie", result[2].Keyword);
        Assert.Equal(3, result[2].CategoryId);
        Assert.NotNull(result[2].Category);
        Assert.Equal(3, result[2].Category.Id);
        Assert.Equal("Entertainment", result[2].Category.Name);
        Assert.Equal("Movies and games", result[2].Category.Description);

        _categoryRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_GetCategoryRules_WithMissingCategory_ShouldReturnRuleWithNullCategory()
    {
        // Arrange
        var rules = new List<CategoryRule>
        {
            new CategoryRule { Id = 1, Keyword = "grocery", CategoryId = 1 },
            new CategoryRule { Id = 2, Keyword = "gas", CategoryId = 999 } // Non-existent category
        };

        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Groceries", Description = "Food shopping" }
        };

        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(rules);
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

        var request = new GetCategoryRules();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify rule with existing category
        Assert.Equal(1, result[0].Id);
        Assert.Equal("grocery", result[0].Keyword);
        Assert.Equal(1, result[0].CategoryId);
        Assert.NotNull(result[0].Category);
        Assert.Equal("Groceries", result[0].Category.Name);

        // Verify rule with missing category
        Assert.Equal(2, result[1].Id);
        Assert.Equal("gas", result[1].Keyword);
        Assert.Equal(999, result[1].CategoryId);
        Assert.Null(result[1].Category);

        _categoryRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_GetCategoryRules_WithEmptyRules_ShouldReturnEmptyList()
    {
        // Arrange
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());

        var request = new GetCategoryRules();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _categoryRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_GetCategoryRules_WithEmptyCategories_ShouldReturnRulesWithNullCategories()
    {
        // Arrange
        var rules = new List<CategoryRule>
        {
            new CategoryRule { Id = 1, Keyword = "grocery", CategoryId = 1 },
            new CategoryRule { Id = 2, Keyword = "gas", CategoryId = 2 }
        };

        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(rules);
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());

        var request = new GetCategoryRules();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify all rules have null categories
        Assert.Equal(1, result[0].Id);
        Assert.Equal("grocery", result[0].Keyword);
        Assert.Equal(1, result[0].CategoryId);
        Assert.Null(result[0].Category);

        Assert.Equal(2, result[1].Id);
        Assert.Equal("gas", result[1].Keyword);
        Assert.Equal(2, result[1].CategoryId);
        Assert.Null(result[1].Category);

        _categoryRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_GetCategoryRules_WithNullKeyword_ShouldHandleCorrectly()
    {
        // Arrange
        var rules = new List<CategoryRule>
        {
            new CategoryRule { Id = 1, Keyword = null, CategoryId = 1 }
        };

        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Test Category", Description = null }
        };

        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(rules);
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

        var request = new GetCategoryRules();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Null(result[0].Keyword);
        Assert.Equal(1, result[0].CategoryId);
        Assert.NotNull(result[0].Category);
        Assert.Equal("Test Category", result[0].Category.Name);
        Assert.Null(result[0].Category.Description);

        _categoryRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
} 