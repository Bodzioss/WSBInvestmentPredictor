using WSBInvestmentPredictor.Expenses.Models;

namespace WSBInvestmentPredictor.Expenses.Services;

public class TransactionStore : ITransactionStore
{
    private readonly List<BankTransaction> _transactions = new();

    public Task AddTransactions(IEnumerable<BankTransaction> transactions)
    {
        _transactions.AddRange(transactions);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<BankTransaction>> GetAllTransactions()
    {
        return Task.FromResult(_transactions.AsEnumerable());
    }

    public Task<IEnumerable<BankTransaction>> GetTransactionsByYear(int year)
    {
        var result = _transactions
            .Where(t => t.TransactionDate.Year == year)
            .OrderByDescending(t => t.TransactionDate)
            .ToList()
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<BankTransaction>> GetTransactionsByYearAndMonth(int year, int month)
    {
        var result = _transactions
            .Where(t => t.TransactionDate.Year == year && t.TransactionDate.Month == month)
            .OrderByDescending(t => t.TransactionDate)
            .ToList()
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<BankTransaction>> GetTransactionsByAccount(string account)
    {
        var result = _transactions
            .Where(t => t.Account == account)
            .OrderByDescending(t => t.TransactionDate)
            .ToList()
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<BankTransaction>> GetTransactionsByCounterparty(string counterparty)
    {
        var result = _transactions
            .Where(t => t.Counterparty == counterparty)
            .OrderByDescending(t => t.TransactionDate)
            .ToList()
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<string>> GetAllAccounts()
    {
        var result = _transactions
            .Select(t => t.Account)
            .Where(a => !string.IsNullOrEmpty(a))
            .Distinct()
            .OrderBy(a => a)
            .ToList()
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<string>> GetAllCounterparties()
    {
        var result = _transactions
            .Select(t => t.Counterparty)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList()
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<int>> GetAllYears()
    {
        var result = _transactions
            .Select(t => t.TransactionDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList()
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<decimal> GetTotalAmountByYear(int year)
    {
        var result = _transactions
            .Where(t => t.TransactionDate.Year == year)
            .Sum(t => t.Amount);
        return Task.FromResult(result);
    }

    public Task<decimal> GetTotalAmountByYearAndMonth(int year, int month)
    {
        var result = _transactions
            .Where(t => t.TransactionDate.Year == year && t.TransactionDate.Month == month)
            .Sum(t => t.Amount);
        return Task.FromResult(result);
    }
} 