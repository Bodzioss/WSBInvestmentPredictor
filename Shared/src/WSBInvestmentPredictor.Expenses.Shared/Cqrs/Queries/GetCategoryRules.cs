using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Expenses.Shared.Dto;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
[ApiRequest("/api/expenses/categoryrules", "POST")]
public record GetCategoryRules() : IRequest<List<CategoryRuleDto>>; 