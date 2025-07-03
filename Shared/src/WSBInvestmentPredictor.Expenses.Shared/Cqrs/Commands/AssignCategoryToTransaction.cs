using MediatR;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

public record AssignCategoryToTransaction(int TransactionId, int CategoryId) : IRequest; 