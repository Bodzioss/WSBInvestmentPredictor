using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Services;

/// <summary>
/// Service interface for managing bank transactions in the data store.
/// Provides methods for storing, retrieving, and aggregating transaction data.
/// </summary>
public interface ITransactionStore
{
    /// <summary>
    /// Adds a collection of bank transactions to the data store.
    /// </summary>
    /// <param name="transactions">The collection of transactions to add</param>
    Task AddTransactions(IEnumerable<BankTransaction> transactions);

    /// <summary>
    /// Retrieves all bank transactions from the data store.
    /// </summary>
    /// <returns>A collection of all bank transactions</returns>
    Task<IEnumerable<BankTransaction>> GetAllTransactions();

    /// <summary>
    /// Retrieves all bank transactions for a specific year.
    /// </summary>
    /// <param name="year">The year to filter transactions by</param>
    /// <returns>A collection of bank transactions for the specified year</returns>
    Task<IEnumerable<BankTransaction>> GetTransactionsByYear(int year);

    /// <summary>
    /// Retrieves all bank transactions for a specific year and month.
    /// </summary>
    /// <param name="year">The year to filter transactions by</param>
    /// <param name="month">The month to filter transactions by</param>
    /// <returns>A collection of bank transactions for the specified year and month</returns>
    Task<IEnumerable<BankTransaction>> GetTransactionsByYearAndMonth(int year, int month);

    /// <summary>
    /// Retrieves all bank transactions for a specific account.
    /// </summary>
    /// <param name="account">The account to filter transactions by</param>
    /// <returns>A collection of bank transactions for the specified account</returns>
    Task<IEnumerable<BankTransaction>> GetTransactionsByAccount(string account);

    /// <summary>
    /// Retrieves all bank transactions for a specific counterparty.
    /// </summary>
    /// <param name="counterparty">The counterparty to filter transactions by</param>
    /// <returns>A collection of bank transactions for the specified counterparty</returns>
    Task<IEnumerable<BankTransaction>> GetTransactionsByCounterparty(string counterparty);

    /// <summary>
    /// Retrieves all unique account identifiers from the data store.
    /// </summary>
    /// <returns>A collection of unique account identifiers</returns>
    Task<IEnumerable<string>> GetAllAccounts();

    /// <summary>
    /// Retrieves all unique counterparty identifiers from the data store.
    /// </summary>
    /// <returns>A collection of unique counterparty identifiers</returns>
    Task<IEnumerable<string>> GetAllCounterparties();

    /// <summary>
    /// Retrieves all unique years from the data store.
    /// </summary>
    /// <returns>A collection of unique years</returns>
    Task<IEnumerable<int>> GetAllYears();

    /// <summary>
    /// Calculates the total transaction amount for a specific year.
    /// </summary>
    /// <param name="year">The year to calculate the total amount for</param>
    /// <returns>The total transaction amount for the specified year</returns>
    Task<decimal> GetTotalAmountByYear(int year);

    /// <summary>
    /// Calculates the total transaction amount for a specific year and month.
    /// </summary>
    /// <param name="year">The year to calculate the total amount for</param>
    /// <param name="month">The month to calculate the total amount for</param>
    /// <returns>The total transaction amount for the specified year and month</returns>
    Task<decimal> GetTotalAmountByYearAndMonth(int year, int month);
}