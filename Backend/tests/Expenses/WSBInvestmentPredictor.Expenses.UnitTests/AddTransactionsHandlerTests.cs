using MediatR;
using Moq;
// using System.Transactions;
using WSBInvestmentPredictor.Expenses.Application.Commands;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using Xunit;

namespace WSBInvestmentPredictor.Expenses.UnitTests;

public class AddTransactionsHandlerTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly AddTransactionsHandler _handler;

    public AddTransactionsHandlerTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _handler = new AddTransactionsHandler(_transactionRepositoryMock.Object);
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

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _transactionRepositoryMock.Verify(x => x.AddTransactions(transactions), Times.Once);
    }
} 