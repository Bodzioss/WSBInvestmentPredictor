using MediatR;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

/// <summary>
/// Command for adding multiple bank transactions to the system.
/// This command is used to persist a collection of transactions in the database.
/// </summary>
/// <param name="Transactions">The collection of bank transactions to be added.</param>
[ApiRequest("/api/transactions/add", "POST")]
public record AddTransactions(IEnumerable<BankTransaction> Transactions) : IRequest;