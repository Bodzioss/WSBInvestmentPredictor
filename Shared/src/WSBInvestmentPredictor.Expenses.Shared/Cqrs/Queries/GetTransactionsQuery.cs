using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;

/// <summary>
/// Query for retrieving bank transactions with optional filtering criteria.
/// This query is used to fetch transactions from the database based on various filter parameters.
/// </summary>
/// <param name="Year">Optional year filter for transactions.</param>
/// <param name="Month">Optional month filter for transactions (1-12).</param>
/// <param name="Account">Optional account identifier filter.</param>
/// <param name="Counterparty">Optional counterparty name filter.</param>
[ApiRequest("/api/transactions/query", "POST")]
public record GetTransactions(
    int? Year = null,
    int? Month = null,
    string? Account = null,
    string? Counterparty = null
) : IRequest<GetTransactionsResponse>;