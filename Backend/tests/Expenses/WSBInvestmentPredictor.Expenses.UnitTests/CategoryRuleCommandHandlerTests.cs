using MediatR;
using Moq;
using WSBInvestmentPredictor.Expenses.Application.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using Xunit;

namespace WSBInvestmentPredictor.Expenses.UnitTests;

public class CategoryRuleCommandHandlerTests
{
    private readonly Mock<ICategoryRuleRepository> _categoryRuleRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly CategoryRuleCommandHandler _handler;

    public CategoryRuleCommandHandlerTests()
    {
        _categoryRuleRepositoryMock = new Mock<ICategoryRuleRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        
        _handler = new CategoryRuleCommandHandler(
            _categoryRuleRepositoryMock.Object, 
            _categoryRepositoryMock.Object,
            _transactionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_AddCategoryRule_ShouldAddRuleAndReturnDto()
    {
        // Arrange
        var request = new AddCategoryRule("grocery", 1);
        var category = new Category { Id = 1, Name = "Groceries", Description = "Food shopping" };
        
        _categoryRuleRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CategoryRule>())).Returns(Task.CompletedTask);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.CategoryId)).ReturnsAsync(category);
        
        // Setup mocks for ApplyRulesToTransactions
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Keyword, result.Keyword);
        Assert.Equal(request.CategoryId, result.CategoryId);
        Assert.NotNull(result.Category);
        Assert.Equal(category.Id, result.Category.Id);
        Assert.Equal(category.Name, result.Category.Name);
        Assert.Equal(category.Description, result.Category.Description);

        _categoryRuleRepositoryMock.Verify(x => x.AddAsync(It.Is<CategoryRule>(r => 
            r.Keyword == request.Keyword && r.CategoryId == request.CategoryId)), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(request.CategoryId), Times.Once);
    }

    [Fact]
    public async Task Handle_AddCategoryRule_WithNonExistentCategory_ShouldReturnDtoWithNullCategory()
    {
        // Arrange
        var request = new AddCategoryRule("grocery", 999);
        
        _categoryRuleRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CategoryRule>())).Returns(Task.CompletedTask);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.CategoryId)).ReturnsAsync((Category?)null);
        
        // Setup mocks for ApplyRulesToTransactions
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Keyword, result.Keyword);
        Assert.Equal(request.CategoryId, result.CategoryId);
        Assert.Null(result.Category);

        _categoryRuleRepositoryMock.Verify(x => x.AddAsync(It.Is<CategoryRule>(r => 
            r.Keyword == request.Keyword && r.CategoryId == request.CategoryId)), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(request.CategoryId), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCategoryRule_ShouldUpdateRuleAndReturnDto()
    {
        // Arrange
        var request = new UpdateCategoryRule(1, "updated grocery", 2);
        var category = new Category { Id = 2, Name = "Updated Groceries", Description = "Updated food shopping" };
        
        _categoryRuleRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CategoryRule>())).Returns(Task.CompletedTask);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.CategoryId)).ReturnsAsync(category);
        
        // Setup mocks for ApplyRulesToTransactions
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Id, result.Id);
        Assert.Equal(request.Keyword, result.Keyword);
        Assert.Equal(request.CategoryId, result.CategoryId);
        Assert.NotNull(result.Category);
        Assert.Equal(category.Id, result.Category.Id);
        Assert.Equal(category.Name, result.Category.Name);
        Assert.Equal(category.Description, result.Category.Description);

        _categoryRuleRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CategoryRule>(r => 
            r.Id == request.Id && r.Keyword == request.Keyword && r.CategoryId == request.CategoryId)), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(request.CategoryId), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCategoryRule_WithNonExistentCategory_ShouldReturnDtoWithNullCategory()
    {
        // Arrange
        var request = new UpdateCategoryRule(1, "updated grocery", 999);
        
        _categoryRuleRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CategoryRule>())).Returns(Task.CompletedTask);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.CategoryId)).ReturnsAsync((Category?)null);
        
        // Setup mocks for ApplyRulesToTransactions
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Id, result.Id);
        Assert.Equal(request.Keyword, result.Keyword);
        Assert.Equal(request.CategoryId, result.CategoryId);
        Assert.Null(result.Category);

        _categoryRuleRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CategoryRule>(r => 
            r.Id == request.Id && r.Keyword == request.Keyword && r.CategoryId == request.CategoryId)), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(request.CategoryId), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCategoryRule_ShouldDeleteRule()
    {
        // Arrange
        var request = new DeleteCategoryRule(1);
        _categoryRuleRepositoryMock.Setup(x => x.DeleteAsync(request.Id)).Returns(Task.CompletedTask);
        
        // Setup mocks for ApplyRulesToTransactions
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction>());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _categoryRuleRepositoryMock.Verify(x => x.DeleteAsync(request.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_AddCategoryRule_WithEmptyKeyword_ShouldAddRule()
    {
        // Arrange
        var request = new AddCategoryRule("", 1);
        var category = new Category { Id = 1, Name = "Test Category", Description = "Test Description" };
        
        _categoryRuleRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CategoryRule>())).Returns(Task.CompletedTask);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.CategoryId)).ReturnsAsync(category);
        
        // Setup mocks for ApplyRulesToTransactions
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("", result.Keyword);
        Assert.Equal(request.CategoryId, result.CategoryId);
        Assert.NotNull(result.Category);

        _categoryRuleRepositoryMock.Verify(x => x.AddAsync(It.Is<CategoryRule>(r => 
            r.Keyword == "" && r.CategoryId == request.CategoryId)), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCategoryRule_WithNullKeyword_ShouldUpdateRule()
    {
        // Arrange
        var request = new UpdateCategoryRule(1, null, 1);
        var category = new Category { Id = 1, Name = "Test Category", Description = "Test Description" };
        
        _categoryRuleRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CategoryRule>())).Returns(Task.CompletedTask);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.CategoryId)).ReturnsAsync(category);
        
        // Setup mocks for ApplyRulesToTransactions
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Keyword);
        Assert.Equal(request.CategoryId, result.CategoryId);
        Assert.NotNull(result.Category);

        _categoryRuleRepositoryMock.Verify(x => x.UpdateAsync(It.Is<CategoryRule>(r => 
            r.Id == request.Id && r.Keyword == null && r.CategoryId == request.CategoryId)), Times.Once);
    }

    [Fact]
    public async Task Handle_AddCategoryRule_WithCategoryHavingNullDescription_ShouldHandleCorrectly()
    {
        // Arrange
        var request = new AddCategoryRule("grocery", 1);
        var category = new Category { Id = 1, Name = "Groceries", Description = null };
        
        _categoryRuleRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CategoryRule>())).Returns(Task.CompletedTask);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.CategoryId)).ReturnsAsync(category);
        
        // Setup mocks for ApplyRulesToTransactions
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Keyword, result.Keyword);
        Assert.Equal(request.CategoryId, result.CategoryId);
        Assert.NotNull(result.Category);
        Assert.Equal(category.Id, result.Category.Id);
        Assert.Equal(category.Name, result.Category.Name);
        Assert.Null(result.Category.Description);

        _categoryRuleRepositoryMock.Verify(x => x.AddAsync(It.Is<CategoryRule>(r => 
            r.Keyword == request.Keyword && r.CategoryId == request.CategoryId)), Times.Once);
    }
} 