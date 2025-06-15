using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Services;

public class TransactionStore : ITransactionStore
{
    private readonly ICqrsRequestService _cqrsRequestService;

    public TransactionStore(ICqrsRequestService cqrsRequestService)
    {
        _cqrsRequestService = cqrsRequestService;
    }

    public async Task AddTransactions(IEnumerable<BankTransaction> transactions)
    {
        var command = new AddTransactions(transactions);
        await _cqrsRequestService.Handle(command);
    }

    public async Task<IEnumerable<BankTransaction>> GetAllTransactions()
    {
        var query = new GetTransactions();
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    public async Task<IEnumerable<BankTransaction>> GetTransactionsByYear(int year)
    {
        var query = new GetTransactions(Year: year);
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    public async Task<IEnumerable<BankTransaction>> GetTransactionsByYearAndMonth(int year, int month)
    {
        var query = new GetTransactions(Year: year, Month: month);
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    public async Task<IEnumerable<BankTransaction>> GetTransactionsByAccount(string account)
    {
        var query = new GetTransactions(Account: account);
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    public async Task<IEnumerable<BankTransaction>> GetTransactionsByCounterparty(string counterparty)
    {
        var query = new GetTransactions(Counterparty: counterparty);
        var result = await _cqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
        return result.Transactions;
    }

    public async Task<IEnumerable<string>> GetAllAccounts()
    {
        var transactions = await GetAllTransactions();
        return transactions
            .Select(t => t.Account)
            .Where(a => !string.IsNullOrEmpty(a))
            .Distinct()
            .OrderBy(a => a);
    }

    public async Task<IEnumerable<string>> GetAllCounterparties()
    {
        var transactions = await GetAllTransactions();
        return transactions
            .Select(t => t.Counterparty)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c);
    }

    public async Task<IEnumerable<int>> GetAllYears()
    {
        var transactions = await GetAllTransactions();
        return transactions
            .Select(t => t.TransactionDate.Year)
            .Distinct()
            .OrderByDescending(y => y);
    }

    public async Task<decimal> GetTotalAmountByYear(int year)
    {
        var transactions = await GetTransactionsByYear(year);
        return transactions.Sum(t => t.Amount);
    }

    public async Task<decimal> GetTotalAmountByYearAndMonth(int year, int month)
    {
        var transactions = await GetTransactionsByYearAndMonth(year, month);
        return transactions.Sum(t => t.Amount);
    }
}