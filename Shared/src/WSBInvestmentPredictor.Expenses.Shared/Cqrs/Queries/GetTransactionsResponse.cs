using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;

/// <summary>
/// Response object for the GetTransactions query.
/// Contains the collection of transactions and the total amount of all transactions.
/// </summary>
/// <param name="Transactions">The collection of bank transactions matching the query criteria.</param>
/// <param name="TotalAmount">The sum of amounts for all transactions in the collection.</param>
public record GetTransactionsResponse(
    IEnumerable<BankTransaction> Transactions,
    decimal TotalAmount
);