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

public class CategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ICategoryRuleRepository> _categoryRuleRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly CategoryCommandHandler _handler;

    public CategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _categoryRuleRepositoryMock = new Mock<ICategoryRuleRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _handler = new CategoryCommandHandler(_categoryRepositoryMock.Object, _categoryRuleRepositoryMock.Object, _transactionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_AddCategory_WhenCategoryDoesNotExist_ShouldAddNewCategory()
    {
        // Arrange
        var request = new AddCategory("Test Category", "Test Description");
        _categoryRepositoryMock.Setup(x => x.GetByNameAsync(request.Name)).ReturnsAsync((Category?)null);
        _categoryRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        _categoryRepositoryMock.Verify(x => x.GetByNameAsync(request.Name), Times.Once);
        _categoryRepositoryMock.Verify(x => x.AddAsync(It.Is<Category>(c => c.Name == request.Name && c.Description == request.Description)), Times.Once);
    }

    [Fact]
    public async Task Handle_AddCategory_WhenCategoryExists_ShouldReturnExistingCategory()
    {
        // Arrange
        var request = new AddCategory("Test Category", "Test Description");
        var existingCategory = new Category { Id = 1, Name = "Test Category", Description = "Existing Description" };
        _categoryRepositoryMock.Setup(x => x.GetByNameAsync(request.Name)).ReturnsAsync(existingCategory);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingCategory.Id, result.Id);
        Assert.Equal(existingCategory.Name, result.Name);
        Assert.Equal(existingCategory.Description, result.Description);
        _categoryRepositoryMock.Verify(x => x.GetByNameAsync(request.Name), Times.Once);
        _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateCategory_WhenCategoryExists_ShouldUpdateCategory()
    {
        // Arrange
        var request = new UpdateCategory(1, "Updated Category", "Updated Description");
        var existingCategory = new Category { Id = 1, Name = "Old Name", Description = "Old Description" };
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.Id)).ReturnsAsync(existingCategory);
        _categoryRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Id, result.Id);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(request.Id), Times.Once);
        _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Category>(c => c.Id == request.Id && c.Name == request.Name && c.Description == request.Description)), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCategory_WhenCategoryDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var request = new UpdateCategory(999, "Updated Category", "Updated Description");
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.Id)).ReturnsAsync((Category?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Contains($"Category with ID {request.Id} does not exist", exception.Message);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(request.Id), Times.Once);
        _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeleteCategory_ShouldDeleteCategoryAndRelatedRules()
    {
        // Arrange
        var request = new DeleteCategory(1);
        var rules = new List<CategoryRule>
        {
            new CategoryRule { Id = 1, CategoryId = 1, Keyword = "test1" },
            new CategoryRule { Id = 2, CategoryId = 1, Keyword = "test2" },
            new CategoryRule { Id = 3, CategoryId = 2, Keyword = "test3" } // Different category
        };
        
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(rules);
        _categoryRuleRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        _categoryRepositoryMock.Setup(x => x.DeleteAsync(request.Id)).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _categoryRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRuleRepositoryMock.Verify(x => x.DeleteAsync(1), Times.Once); // First rule
        _categoryRuleRepositoryMock.Verify(x => x.DeleteAsync(2), Times.Once); // Second rule
        _categoryRuleRepositoryMock.Verify(x => x.DeleteAsync(3), Times.Never); // Different category rule
        _categoryRepositoryMock.Verify(x => x.DeleteAsync(request.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_AssignCategoryToTransaction_WhenTransactionExists_ShouldAssignCategory()
    {
        // Arrange
        var request = new AssignCategoryToTransaction(1, 2);
        var transaction = new BankTransaction(DateTime.Now, "Test Transaction", 100m, "Account", "Counterparty") { Id = 1 };
        var category = new Category { Id = 2, Name = "Test Category", Description = "Test Description" };
        
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction> { transaction });
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.CategoryId)).ReturnsAsync(category);
        _transactionRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<BankTransaction>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _transactionRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(request.CategoryId), Times.Once);
        _transactionRepositoryMock.Verify(x => x.UpdateAsync(It.Is<BankTransaction>(t => t.Id == request.TransactionId && t.Category == category.Name)), Times.Once);
    }

    [Fact]
    public async Task Handle_AssignCategoryToTransaction_WhenTransactionDoesNotExist_ShouldDoNothing()
    {
        // Arrange
        var request = new AssignCategoryToTransaction(999, 2);
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction>());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _transactionRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _transactionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<BankTransaction>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AssignCategoryToTransaction_WhenCategoryDoesNotExist_ShouldAssignEmptyCategory()
    {
        // Arrange
        var request = new AssignCategoryToTransaction(1, 999);
        var transaction = new BankTransaction(DateTime.Now, "Test Transaction", 100m, "Account", "Counterparty") { Id = 1 };
        
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BankTransaction> { transaction });
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(request.CategoryId)).ReturnsAsync((Category?)null);
        _transactionRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<BankTransaction>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _transactionRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(request.CategoryId), Times.Once);
        _transactionRepositoryMock.Verify(x => x.UpdateAsync(It.Is<BankTransaction>(t => t.Id == request.TransactionId && t.Category == string.Empty)), Times.Once);
    }

    [Fact]
    public async Task Handle_ApplyCategoryRules_ShouldApplyRulesToMatchingTransactions()
    {
        // Arrange
        var request = new ApplyCategoryRules();
        var rules = new List<CategoryRule>
        {
            new CategoryRule { Id = 1, CategoryId = 1, Keyword = "grocery" },
            new CategoryRule { Id = 2, CategoryId = 2, Keyword = "gas" }
        };
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Groceries", Description = "Food shopping" },
            new Category { Id = 2, Name = "Transportation", Description = "Gas and fuel" }
        };
        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Grocery Store Purchase", 50m, "Account", "Counterparty") { Id = 1 },
            new BankTransaction(DateTime.Now, "Gas Station", 30m, "Account", "Counterparty") { Id = 2 },
            new BankTransaction(DateTime.Now, "Restaurant", 25m, "Account", "Counterparty") { Id = 3 }
        };
        
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(rules);
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(transactions);
        _transactionRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<BankTransaction>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _categoryRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        _transactionRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        
        // Verify that matching transactions were updated
        _transactionRepositoryMock.Verify(x => x.UpdateAsync(It.Is<BankTransaction>(t => t.Id == 1 && t.Category == "Groceries")), Times.Once);
        _transactionRepositoryMock.Verify(x => x.UpdateAsync(It.Is<BankTransaction>(t => t.Id == 2 && t.Category == "Transportation")), Times.Once);
        _transactionRepositoryMock.Verify(x => x.UpdateAsync(It.Is<BankTransaction>(t => t.Id == 3)), Times.Never); // No matching rule
    }

    [Fact]
    public async Task Handle_ApplyCategoryRules_WithEmptyKeyword_ShouldSkipRule()
    {
        // Arrange
        var request = new ApplyCategoryRules();
        var rules = new List<CategoryRule>
        {
            new CategoryRule { Id = 1, CategoryId = 1, Keyword = "" }, // Empty keyword
            new CategoryRule { Id = 2, CategoryId = 2, Keyword = "gas" }
        };
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Groceries", Description = "Food shopping" },
            new Category { Id = 2, Name = "Transportation", Description = "Gas and fuel" }
        };
        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Gas Station", 30m, "Account", "Counterparty") { Id = 1 }
        };
        
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(rules);
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(transactions);
        _transactionRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<BankTransaction>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _transactionRepositoryMock.Verify(x => x.UpdateAsync(It.Is<BankTransaction>(t => t.Id == 1 && t.Category == "Transportation")), Times.Once);
        // Should not update with empty keyword rule
    }

    [Fact]
    public async Task Handle_ApplyCategoryRules_WithNonExistentCategory_ShouldSkipRule()
    {
        // Arrange
        var request = new ApplyCategoryRules();
        var rules = new List<CategoryRule>
        {
            new CategoryRule { Id = 1, CategoryId = 999, Keyword = "grocery" } // Non-existent category
        };
        var categories = new List<Category>(); // Empty categories
        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Grocery Store Purchase", 50m, "Account", "Counterparty") { Id = 1 }
        };
        
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(rules);
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
        _transactionRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _transactionRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<BankTransaction>()), Times.Never); // No updates should occur
    }
} 