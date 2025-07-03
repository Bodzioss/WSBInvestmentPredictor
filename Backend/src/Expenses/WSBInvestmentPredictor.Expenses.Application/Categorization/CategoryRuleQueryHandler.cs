using MediatR;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Dto;

namespace WSBInvestmentPredictor.Expenses.Application.Categorization;

public class CategoryRuleQueryHandler :
    IRequestHandler<GetCategoryRules, List<CategoryRuleDto>>
{
    private readonly ICategoryRuleRepository _ruleRepo;
    private readonly ICategoryRepository _categoryRepo;
    
    public CategoryRuleQueryHandler(ICategoryRuleRepository ruleRepo, ICategoryRepository categoryRepo)
    {
        _ruleRepo = ruleRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<List<CategoryRuleDto>> Handle(GetCategoryRules request, CancellationToken cancellationToken)
    {
        var rules = await _ruleRepo.GetAllAsync();
        var categories = await _categoryRepo.GetAllAsync();
        
        return rules.Select(r => 
        {
            var category = categories.FirstOrDefault(c => c.Id == r.CategoryId);
            return new CategoryRuleDto 
            { 
                Id = r.Id, 
                Keyword = r.Keyword, 
                CategoryId = r.CategoryId,
                FieldType = r.FieldType,
                Category = category != null ? new CategoryDto(category.Id, category.Name, category.Description) : null
            };
        }).ToList();
    }
} 