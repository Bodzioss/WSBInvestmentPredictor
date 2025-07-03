using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

/// <summary>
/// Command for updating a single bank transaction in the system.
/// This command is used to modify an existing transaction in the database.
/// </summary>
/// <param name="Id">The unique identifier of the transaction to update.</param>
/// <param name="Date">The transaction date.</param>
/// <param name="Title">The transaction title.</param>
/// <param name="Counterparty">The transaction counterparty.</param>
/// <param name="Amount">The transaction amount.</param>
/// <param name="Category">The transaction category.</param>
/// <param name="Account">The account number.</param>
/// <param name="Currency">The transaction currency.</param>
[ApiRequest("/api/transactions/update", "PUT")]
public record UpdateTransactionCommand(
    int Id,
    DateTime Date,
    string Title,
    string Counterparty,
    decimal Amount,
    string? Category,
    string Account,
    string Currency) : IRequest; 