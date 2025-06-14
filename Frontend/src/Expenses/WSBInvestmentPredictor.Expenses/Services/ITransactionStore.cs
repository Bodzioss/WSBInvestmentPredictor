using WSBInvestmentPredictor.Expenses.Models;

namespace WSBInvestmentPredictor.Expenses.Services;

public interface ITransactionStore
{
    Task AddTransactions(IEnumerable<BankTransaction> transactions);
    Task<IEnumerable<BankTransaction>> GetAllTransactions();
    Task<IEnumerable<BankTransaction>> GetTransactionsByYear(int year);
    Task<IEnumerable<BankTransaction>> GetTransactionsByYearAndMonth(int year, int month);
    Task<IEnumerable<BankTransaction>> GetTransactionsByAccount(string account);
    Task<IEnumerable<BankTransaction>> GetTransactionsByCounterparty(string counterparty);
    Task<IEnumerable<string>> GetAllAccounts();
    Task<IEnumerable<string>> GetAllCounterparties();
    Task<IEnumerable<int>> GetAllYears();
    Task<decimal> GetTotalAmountByYear(int year);
    Task<decimal> GetTotalAmountByYearAndMonth(int year, int month);
} 