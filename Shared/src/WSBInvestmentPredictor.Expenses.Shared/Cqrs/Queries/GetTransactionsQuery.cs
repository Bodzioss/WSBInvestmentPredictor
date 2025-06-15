using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;

[ApiRequest("/api/transactions/query", "POST")]
public record GetTransactions(
    int? Year = null,
    int? Month = null,
    string? Account = null,
    string? Counterparty = null
) : IRequest<GetTransactionsResponse>;