using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<BankTransaction>> GetTransactions(int? year = null, int? month = null, string? account = null, string? counterparty = null);

    Task<IEnumerable<BankTransaction>> GetAllAsync();

    Task UpdateAsync(BankTransaction transaction);

    Task<IEnumerable<string>> GetAllAccounts();

    Task<IEnumerable<string>> GetAllCounterparties();

    Task<IEnumerable<int>> GetAllYears();

    Task AddTransactions(IEnumerable<BankTransaction> transactions);
}