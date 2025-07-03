using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WSBInvestmentPredictor.Expenses.Infrastructure.Data;
using WSBInvestmentPredictor.Expenses.Infrastructure.Repositories;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using Xunit;

namespace WSBInvestmentPredictor.Expenses.IntegrationTests;

public class TransactionRepositoryTests : IDisposable
{
    private readonly ExpensesDbContext _context;
    private readonly TransactionRepository _repository;

    public TransactionRepositoryTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ExpensesDbContext>(options =>
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
        
        var serviceProvider = services.BuildServiceProvider();
        _context = serviceProvider.GetRequiredService<ExpensesDbContext>();
        _context.Database.EnsureCreated();
        
        _repository = new TransactionRepository(_context);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    [Fact]
    public async Task AddTransactions_ShouldAddTransactionsToRepository()
    {
        // Arrange
        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Test1", 100m, "Account1", "Counterparty1"),
            new BankTransaction(DateTime.Now, "Test2", 200m, "Account2", "Counterparty2")
        };

        // Act
        await _repository.AddTransactions(transactions);

        // Assert
        var result = await _repository.GetTransactions();
        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.Title == "Test1" && t.Amount == 100m);
        Assert.Contains(result, t => t.Title == "Test2" && t.Amount == 200m);
    }

    [Fact]
    public async Task GetTransactions_WithFilters_ShouldReturnFilteredTransactions()
    {
        // Arrange
        var date1 = new DateTime(2024, 3, 1);
        var date2 = new DateTime(2024, 4, 1);
        var transactions = new List<BankTransaction>
        {
            new BankTransaction(date1, "Test1", 100m, "Account1", "Counterparty1"),
            new BankTransaction(date2, "Test2", 200m, "Account2", "Counterparty2")
        };
        await _repository.AddTransactions(transactions);

        // Act
        var result = await _repository.GetTransactions(2024, 3, "Account1", "Counterparty1");

        // Assert
        Assert.Single(result);
        var transaction = result.First();
        Assert.Equal("Test1", transaction.Title);
        Assert.Equal(100m, transaction.Amount);
        Assert.Equal("Account1", transaction.Account);
        Assert.Equal("Counterparty1", transaction.Counterparty);
    }

    [Fact]
    public async Task GetAllAccounts_ShouldReturnAllUniqueAccounts()
    {
        // Arrange
        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Test1", 100m, "Account1", "Counterparty1"),
            new BankTransaction(DateTime.Now, "Test2", 200m, "Account1", "Counterparty2"),
            new BankTransaction(DateTime.Now, "Test3", 300m, "Account2", "Counterparty1")
        };
        await _repository.AddTransactions(transactions);

        // Act
        var accounts = await _repository.GetAllAccounts();

        // Assert
        Assert.Equal(2, accounts.Count());
        Assert.Contains("Account1", accounts);
        Assert.Contains("Account2", accounts);
    }

    [Fact]
    public async Task GetAllCounterparties_ShouldReturnAllUniqueCounterparties()
    {
        // Arrange
        var transactions = new List<BankTransaction>
        {
            new BankTransaction(DateTime.Now, "Test1", 100m, "Account1", "Counterparty1"),
            new BankTransaction(DateTime.Now, "Test2", 200m, "Account2", "Counterparty1"),
            new BankTransaction(DateTime.Now, "Test3", 300m, "Account1", "Counterparty2")
        };
        await _repository.AddTransactions(transactions);

        // Act
        var counterparties = await _repository.GetAllCounterparties();

        // Assert
        Assert.Equal(2, counterparties.Count());
        Assert.Contains("Counterparty1", counterparties);
        Assert.Contains("Counterparty2", counterparties);
    }

    [Fact]
    public async Task GetAllYears_ShouldReturnAllUniqueYears()
    {
        // Arrange
        var transactions = new List<BankTransaction>
        {
            new BankTransaction(new DateTime(2023, 1, 1), "Test1", 100m, "Account1", "Counterparty1"),
            new BankTransaction(new DateTime(2024, 1, 1), "Test2", 200m, "Account2", "Counterparty2"),
            new BankTransaction(new DateTime(2024, 2, 1), "Test3", 300m, "Account1", "Counterparty1")
        };
        await _repository.AddTransactions(transactions);

        // Act
        var years = await _repository.GetAllYears();

        // Assert
        Assert.Equal(2, years.Count());
        Assert.Contains(2023, years);
        Assert.Contains(2024, years);
        Assert.Equal(2024, years.First()); // Should be ordered descending
    }
} 