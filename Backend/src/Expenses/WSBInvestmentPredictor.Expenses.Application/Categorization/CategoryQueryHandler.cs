using MediatR;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Dto;

namespace WSBInvestmentPredictor.Expenses.Application.Categorization;

public class CategoryQueryHandler : IRequestHandler<GetCategories, List<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepo;
    
    public CategoryQueryHandler(ICategoryRepository categoryRepo) => _categoryRepo = categoryRepo;
    
    public async Task<List<CategoryDto>> Handle(GetCategories request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepo.GetAllAsync();
        return categories
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description))
            .OrderBy(x => x.Name)
            .ToList();
    }
} 