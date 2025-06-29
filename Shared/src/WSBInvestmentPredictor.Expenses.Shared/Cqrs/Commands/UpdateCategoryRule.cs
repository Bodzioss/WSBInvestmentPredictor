using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using WSBInvestmentPredictor.Expenses.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

[ApiRequest("/api/expenses/categorierules", "PUT")]
public record UpdateCategoryRule(
    int Id,
    [Required(ErrorMessage = "Keyword is required for category rule.")]
    string Keyword, 
    [Required(ErrorMessage = "Category is required for category rule.")]
    int CategoryId,
    [Required(ErrorMessage = "Field type is required for category rule.")]
    TransactionFieldType FieldType = TransactionFieldType.Title) : IRequest<CategoryRuleDto>; 