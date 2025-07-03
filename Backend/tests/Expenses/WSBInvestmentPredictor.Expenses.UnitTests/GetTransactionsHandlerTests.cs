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
    private readonly TransactionQueryHandler _handler;

    public GetTransactionsHandlerTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _handler = new TransactionQueryHandler(_transactionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_GetTransactions_ShouldReturnCorrectResponse()
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
    public async Task Handle_GetTransactions_WithNullFilters_ShouldReturnCorrectResponse()
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

    [Fact]
    public async Task Handle_GetTransactions_WithAllNullFilters_ShouldReturnCorrectResponse()
    {
        // Arrange
        int? year = null;
        int? month = null;
        string? account = null;
        string? counterparty = null;

        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Test1", 100m, "Account1", "Counterparty1"),
            new BankTransaction(DateTime.Now, "Test2", 200m, "Account2", "Counterparty2"),
            new BankTransaction(DateTime.Now, "Test3", 300m, "Account3", "Counterparty3")
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
        Assert.Equal(600m, result.TotalAmount);
        _transactionRepositoryMock.Verify(x => x.GetTransactions(year, month, account, counterparty), Times.Once);
    }

    [Fact]
    public async Task Handle_GetTransactions_WithEmptyTransactions_ShouldReturnEmptyResponse()
    {
        // Arrange
        var year = 2024;
        var month = 3;
        var account = "TestAccount";
        var counterparty = "TestCounterparty";

        var transactions = new List<BankTransaction>();

        _transactionRepositoryMock
            .Setup(x => x.GetTransactions(year, month, account, counterparty))
            .ReturnsAsync(transactions);

        var query = new GetTransactions(year, month, account, counterparty);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Transactions);
        Assert.Equal(0m, result.TotalAmount);
        _transactionRepositoryMock.Verify(x => x.GetTransactions(year, month, account, counterparty), Times.Once);
    }

    [Fact]
    public async Task Handle_GetTransactions_WithZeroAmounts_ShouldCalculateCorrectly()
    {
        // Arrange
        var year = 2024;
        var month = 3;
        var account = "TestAccount";
        var counterparty = "TestCounterparty";

        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Test1", 0m, account, counterparty),
            new BankTransaction(DateTime.Now, "Test2", 0m, account, counterparty),
            new BankTransaction(DateTime.Now, "Test3", 0m, account, counterparty)
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
        Assert.Equal(0m, result.TotalAmount);
        _transactionRepositoryMock.Verify(x => x.GetTransactions(year, month, account, counterparty), Times.Once);
    }

    [Fact]
    public async Task Handle_GetUncategorizedTransactions_ShouldReturnOnlyUncategorizedTransactions()
    {
        // Arrange
        var allTransactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Grocery Store", 50m, "Account1", "Store1") { Category = "Groceries" },
            new BankTransaction(DateTime.Now, "Gas Station", 30m, "Account1", "Station1") { Category = "Transportation" },
            new BankTransaction(DateTime.Now, "Unknown Transaction", 25m, "Account1", "Unknown1") { Category = null },
            new BankTransaction(DateTime.Now, "Another Unknown", 15m, "Account1", "Unknown2") { Category = "" },
            new BankTransaction(DateTime.Now, "Restaurant", 40m, "Account1", "Restaurant1") { Category = "Food" }
        };

        _transactionRepositoryMock
            .Setup(x => x.GetTransactions(null, null, null, null))
            .ReturnsAsync(allTransactions);

        var query = new GetUncategorizedTransactions();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        
        // Verify only uncategorized transactions are returned
        Assert.Contains(result, t => t.Title == "Unknown Transaction" && t.Category == null);
        Assert.Contains(result, t => t.Title == "Another Unknown" && t.Category == "");
        
        // Verify categorized transactions are not included
        Assert.DoesNotContain(result, t => t.Category == "Groceries");
        Assert.DoesNotContain(result, t => t.Category == "Transportation");
        Assert.DoesNotContain(result, t => t.Category == "Food");

        _transactionRepositoryMock.Verify(x => x.GetTransactions(null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task Handle_GetUncategorizedTransactions_WithAllCategorized_ShouldReturnEmptyList()
    {
        // Arrange
        var allTransactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Grocery Store", 50m, "Account1", "Store1") { Category = "Groceries" },
            new BankTransaction(DateTime.Now, "Gas Station", 30m, "Account1", "Station1") { Category = "Transportation" },
            new BankTransaction(DateTime.Now, "Restaurant", 40m, "Account1", "Restaurant1") { Category = "Food" }
        };

        _transactionRepositoryMock
            .Setup(x => x.GetTransactions(null, null, null, null))
            .ReturnsAsync(allTransactions);

        var query = new GetUncategorizedTransactions();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _transactionRepositoryMock.Verify(x => x.GetTransactions(null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task Handle_GetUncategorizedTransactions_WithAllUncategorized_ShouldReturnAllTransactions()
    {
        // Arrange
        var allTransactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Unknown1", 50m, "Account1", "Unknown1") { Category = null },
            new BankTransaction(DateTime.Now, "Unknown2", 30m, "Account1", "Unknown2") { Category = "" },
            new BankTransaction(DateTime.Now, "Unknown3", 40m, "Account1", "Unknown3") { Category = "   " }
        };

        _transactionRepositoryMock
            .Setup(x => x.GetTransactions(null, null, null, null))
            .ReturnsAsync(allTransactions);

        var query = new GetUncategorizedTransactions();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count); // null, empty string, and whitespace-only are all considered uncategorized
        Assert.Contains(result, t => t.Title == "Unknown1" && t.Category == null);
        Assert.Contains(result, t => t.Title == "Unknown2" && t.Category == "");
        Assert.Contains(result, t => t.Title == "Unknown3" && t.Category == "   ");
        _transactionRepositoryMock.Verify(x => x.GetTransactions(null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task Handle_GetUncategorizedTransactions_WithEmptyRepository_ShouldReturnEmptyList()
    {
        // Arrange
        _transactionRepositoryMock
            .Setup(x => x.GetTransactions(null, null, null, null))
            .ReturnsAsync(new List<BankTransaction>());

        var query = new GetUncategorizedTransactions();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _transactionRepositoryMock.Verify(x => x.GetTransactions(null, null, null, null), Times.Once);
    }
} 