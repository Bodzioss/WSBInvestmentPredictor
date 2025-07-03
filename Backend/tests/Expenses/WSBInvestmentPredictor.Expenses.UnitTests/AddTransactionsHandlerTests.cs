using MediatR;
using Moq;
// using System.Transactions;
using WSBInvestmentPredictor.Expenses.Application.Commands;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using Xunit;

namespace WSBInvestmentPredictor.Expenses.UnitTests;

public class AddTransactionsHandlerTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<ICategoryRuleRepository> _categoryRuleRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly AddTransactionsHandler _handler;

    public AddTransactionsHandlerTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _categoryRuleRepositoryMock = new Mock<ICategoryRuleRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        
        _handler = new AddTransactionsHandler(
            _transactionRepositoryMock.Object,
            _categoryRuleRepositoryMock.Object,
            _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryAddTransactions()
    {
        // Arrange
        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Test", 100m, "Account1", "Counterparty1")
        };
        var command = new AddTransactions(transactions);

        // Setup mocks for rule application
        _categoryRuleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryRule>());
        _categoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _transactionRepositoryMock.Verify(x => x.AddTransactions(transactions), Times.Once);
    }
} 