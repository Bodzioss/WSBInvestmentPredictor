using Microsoft.EntityFrameworkCore;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Infrastructure.Data;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ExpensesDbContext _context;

    public TransactionRepository(ExpensesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BankTransaction>> GetTransactions(int? year = null, int? month = null, string? account = null, string? counterparty = null)
    {
        var query = _context.Transactions.AsQueryable();

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

        return await query.OrderByDescending(t => t.TransactionDate).ToListAsync();
    }

    public async Task<IEnumerable<BankTransaction>> GetAllAsync()
    {
        return await _context.Transactions.OrderByDescending(t => t.TransactionDate).ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAllAccounts()
    {
        return await _context.Transactions
            .Select(t => t.Account)
            .Where(a => !string.IsNullOrEmpty(a))
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAllCounterparties()
    {
        return await _context.Transactions
            .Select(t => t.Counterparty)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<IEnumerable<int>> GetAllYears()
    {
        return await _context.Transactions
            .Select(t => t.TransactionDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();
    }

    public async Task AddTransactions(IEnumerable<BankTransaction> transactions)
    {
        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
    }

    public async Task Clear()
    {
        _context.Transactions.RemoveRange(_context.Transactions);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BankTransaction transaction)
    {
        // Check if the entity is already being tracked
        var existingEntity = _context.ChangeTracker.Entries<BankTransaction>()
            .FirstOrDefault(e => e.Entity.Id == transaction.Id);

        if (existingEntity != null)
        {
            // Entity is already tracked, just save changes
            await _context.SaveChangesAsync();
        }
        else
        {
            // Entity is not tracked, attach and update
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}