using MediatR;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using System.ComponentModel.DataAnnotations;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

[ApiRequest("/api/expenses/categories", "PUT")]
public record UpdateCategory(
    int Id,
    [Required(ErrorMessage = "Category name is required.")]
    string Name, 
    string? Description = null) : IRequest<CategoryDto>; 