using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Expenses.Shared.Dto;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;

[ApiRequest("/api/expenses/categories", "GET")]
public record GetCategories() : IRequest<List<CategoryDto>>; 