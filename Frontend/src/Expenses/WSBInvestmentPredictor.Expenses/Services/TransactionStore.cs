using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Services;

/// <summary>
/// Implementation of the transaction store service that uses CQRS pattern for data operations.
/// Handles storing, retrieving, and aggregating bank transactions through command and query handlers.
/// </summary>
public class TransactionStore : ITransactionStore
{
    private readonly ICqrsRequestService _cqrsRequestService;

    /// <summary>
    /// Initializes a new instance of the TransactionStore with the required CQRS request service.
    /// </summary>
    /// <param name="cqrsRequestService">The CQRS request service for handling commands and queries</param>
    public TransactionStore(ICqrsRequestService cqrsRequestService)
    {
        _cqrsRequestService = cqrsRequestService;
    }

    /// <summary>
    /// Adds a collection of bank transactions to the data store using the AddTransactions command.
    /// </summary>
    /// <param name="transactions">The collection of transactions to add</param>
    public async Task AddTransactions(IEnumerable<BankTransaction> transactions)
    {
        var command = new AddTransactions(transactions);
        await _cqrsRequestService.Handle(command);
    }

    /// <summary>
    /// Retrieves all bank transactions using the GetTransactions query.
    /// </summary>
    /// <returns>A collection of all bank transactions</returns>
    public async Task<IEnumerable<BankTransaction>> GetAllTransactions()
    {
        var query = new GetTransactions();
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    /// <summary>
    /// Retrieves all bank transactions for a specific year using the GetTransactions query.
    /// </summary>
    /// <param name="year">The year to filter transactions by</param>
    /// <returns>A collection of bank transactions for the specified year</returns>
    public async Task<IEnumerable<BankTransaction>> GetTransactionsByYear(int year)
    {
        var query = new GetTransactions(Year: year);
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    /// <summary>
    /// Retrieves all bank transactions for a specific year and month using the GetTransactions query.
    /// </summary>
    /// <param name="year">The year to filter transactions by</param>
    /// <param name="month">The month to filter transactions by</param>
    /// <returns>A collection of bank transactions for the specified year and month</returns>
    public async Task<IEnumerable<BankTransaction>> GetTransactionsByYearAndMonth(int year, int month)
    {
        var query = new GetTransactions(Year: year, Month: month);
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    /// <summary>
    /// Retrieves all bank transactions for a specific account using the GetTransactions query.
    /// </summary>
    /// <param name="account">The account to filter transactions by</param>
    /// <returns>A collection of bank transactions for the specified account</returns>
    public async Task<IEnumerable<BankTransaction>> GetTransactionsByAccount(string account)
    {
        var query = new GetTransactions(Account: account);
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    /// <summary>
    /// Retrieves all bank transactions for a specific counterparty using the GetTransactions query.
    /// </summary>
    /// <param name="counterparty">The counterparty to filter transactions by</param>
    /// <returns>A collection of bank transactions for the specified counterparty</returns>
    public async Task<IEnumerable<BankTransaction>> GetTransactionsByCounterparty(string counterparty)
    {
        var query = new GetTransactions(Counterparty: counterparty);
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    /// <summary>
    /// Retrieves all unique account identifiers by querying all transactions and extracting distinct account values.
    /// </summary>
    /// <returns>A collection of unique account identifiers</returns>
    public async Task<IEnumerable<string>> GetAllAccounts()
    {
        var transactions = await GetAllTransactions();
        return transactions
            .Select(t => t.Account)
            .Where(a => !string.IsNullOrEmpty(a))
            .Distinct()
            .OrderBy(a => a);
    }

    /// <summary>
    /// Retrieves all unique counterparty identifiers by querying all transactions and extracting distinct counterparty values.
    /// </summary>
    /// <returns>A collection of unique counterparty identifiers</returns>
    public async Task<IEnumerable<string>> GetAllCounterparties()
    {
        var transactions = await GetAllTransactions();
        return transactions
            .Select(t => t.Counterparty)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c);
    }

    /// <summary>
    /// Retrieves all unique years by querying all transactions and extracting distinct transaction years.
    /// </summary>
    /// <returns>A collection of unique years</returns>
    public async Task<IEnumerable<int>> GetAllYears()
    {
        var transactions = await GetAllTransactions();
        return transactions
            .Select(t => t.TransactionDate.Year)
            .Distinct()
            .OrderByDescending(y => y);
    }

    /// <summary>
    /// Calculates the total transaction amount for a specific year by summing the amounts of all transactions in that year.
    /// </summary>
    /// <param name="year">The year to calculate the total amount for</param>
    /// <returns>The total transaction amount for the specified year</returns>
    public async Task<decimal> GetTotalAmountByYear(int year)
    {
        var transactions = await GetTransactionsByYear(year);
        return transactions.Sum(t => t.Amount);
    }

    /// <summary>
    /// Calculates the total transaction amount for a specific year and month by summing the amounts of all transactions in that period.
    /// </summary>
    /// <param name="year">The year to calculate the total amount for</param>
    /// <param name="month">The month to calculate the total amount for</param>
    /// <returns>The total transaction amount for the specified year and month</returns>
    public async Task<decimal> GetTotalAmountByYearAndMonth(int year, int month)
    {
        var transactions = await GetTransactionsByYearAndMonth(year, month);
        return transactions.Sum(t => t.Amount);
    }
}