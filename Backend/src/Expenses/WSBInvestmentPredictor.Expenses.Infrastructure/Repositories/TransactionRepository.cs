using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private static readonly List<BankTransaction> _transactions = new();
    private static int _nextId = 1;

    public async Task<IEnumerable<BankTransaction>> GetTransactions(int? year = null, int? month = null, string? account = null, string? counterparty = null)
    {
        var query = _transactions.AsQueryable();

        if (year.HasValue)
        {
            query = query.Where(t => t.TransactionDate.Year == year.Value);
            if (month.HasValue)
            {
                query = query.Where(t => t.TransactionDate.Month == month.Value);
            }
        }

        if (!string.IsNullOrEmpty(account))
        {
            query = query.Where(t => t.Account == account);
        }

        if (!string.IsNullOrEmpty(counterparty))
        {
            query = query.Where(t => t.Counterparty == counterparty);
        }

        return query.OrderByDescending(t => t.TransactionDate).ToList();
    }

    public async Task<IEnumerable<BankTransaction>> GetAllAsync()
    {
        return _transactions.OrderByDescending(t => t.TransactionDate).ToList();
    }

    public async Task<IEnumerable<string>> GetAllAccounts()
    {
        return _transactions
            .Select(t => t.Account)
            .Where(a => !string.IsNullOrEmpty(a))
            .Distinct()
            .OrderBy(a => a)
            .ToList();
    }

    public async Task<IEnumerable<string>> GetAllCounterparties()
    {
        return _transactions
            .Select(t => t.Counterparty)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public async Task<IEnumerable<int>> GetAllYears()
    {
        return _transactions
            .Select(t => t.TransactionDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();
    }

    public async Task AddTransactions(IEnumerable<BankTransaction> transactions)
    {
        foreach (var tx in transactions)
        {
            tx.Id = _nextId++;
            _transactions.Add(tx);
        }
    }

    public void Clear()
    {
        _transactions.Clear();
    }

    public async Task UpdateAsync(BankTransaction transaction)
    {
        var existingTransaction = _transactions.FirstOrDefault(t => t.Id == transaction.Id);
        if (existingTransaction != null)
        {
            var index = _transactions.IndexOf(existingTransaction);
            _transactions[index] = transaction;
        }
    }
}