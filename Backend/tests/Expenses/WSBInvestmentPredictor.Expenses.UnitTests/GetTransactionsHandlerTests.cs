using MediatR;
using Moq;
// using System.Transactions;
using WSBInvestmentPredictor.Expenses.Application.Queries;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using Xunit;

namespace WSBInvestmentPredictor.Expenses.UnitTests;

public class GetTransactionsHandlerTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly GetTransactionsHandler _handler;

    public GetTransactionsHandlerTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _handler = new GetTransactionsHandler(_transactionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectResponse()
    {
        // Arrange
        var year = 2024;
        var month = 3;
        var account = "TestAccount";
        var counterparty = "TestCounterparty";

        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Test1", 100m, account, counterparty),
            new BankTransaction(DateTime.Now, "Test2", 200m, account, counterparty)
        };

        _transactionRepositoryMock
            .Setup(x => x.GetTransactions(year, month, account, counterparty))
            .ReturnsAsync(transactions);

        var query = new GetTransactions(year, month, account, counterparty);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactions, result.Transactions);
        Assert.Equal(300m, result.TotalAmount);
        _transactionRepositoryMock.Verify(x => x.GetTransactions(year, month, account, counterparty), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullFilters_ShouldReturnCorrectResponse()
    {
        // Arrange
        var year = 2024;
        var month = 3;
        string? account = null;
        string? counterparty = null;

        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Test1", 100m, "Account1", "Counterparty1"),
            new BankTransaction(DateTime.Now, "Test2", 200m, "Account2", "Counterparty2")
        };

        _transactionRepositoryMock
            .Setup(x => x.GetTransactions(year, month, account, counterparty))
            .ReturnsAsync(transactions);

        var query = new GetTransactions(year, month, account, counterparty);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactions, result.Transactions);
        Assert.Equal(300m, result.TotalAmount);
        _transactionRepositoryMock.Verify(x => x.GetTransactions(year, month, account, counterparty), Times.Once);
    }
} 