using MediatR;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

[ApiRequest("/api/transactions/add", "POST")]
public record AddTransactions(IEnumerable<BankTransaction> Transactions) : IRequest;