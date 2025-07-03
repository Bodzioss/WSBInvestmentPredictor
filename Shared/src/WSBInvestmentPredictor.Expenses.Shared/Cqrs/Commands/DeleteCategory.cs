using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

[ApiRequest("/api/expenses/categories", "DELETE")]
public record DeleteCategory(int Id) : IRequest; 