using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

/// <summary>
/// Command for clearing all bank transactions from the system.
/// This command is used to remove all transactions from the database.
/// </summary>
[ApiRequest("/api/transactions/clear", "DELETE")]
public record ClearAllTransactions() : IRequest; 