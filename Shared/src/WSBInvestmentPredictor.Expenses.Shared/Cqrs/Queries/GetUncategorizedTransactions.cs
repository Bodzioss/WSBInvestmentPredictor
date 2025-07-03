using MediatR;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;

[ApiRequest("/api/expenses/transactions/uncategorized", "POST")]
public record GetUncategorizedTransactions() : IRequest<List<BankTransaction>>; 