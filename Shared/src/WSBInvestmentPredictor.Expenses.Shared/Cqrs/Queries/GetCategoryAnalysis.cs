using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using System.Collections.Generic;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;

[ApiRequest("/api/expenses/category-analysis", "GET")]
public record GetCategoryAnalysis() : IRequest<List<CategoryAnalysisDto>>; 