using MediatR;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Dto;

namespace WSBInvestmentPredictor.Expenses.Application.Categorization;

public class CategoryRuleCommandHandler :
    IRequestHandler<AddCategoryRule, CategoryRuleDto>,
    IRequestHandler<UpdateCategoryRule, CategoryRuleDto>,
    IRequestHandler<DeleteCategoryRule>
{
    private readonly ICategoryRuleRepository _ruleRepo;
    private readonly ICategoryRepository _categoryRepo;
    
    public CategoryRuleCommandHandler(ICategoryRuleRepository ruleRepo, ICategoryRepository categoryRepo)
    {
        _ruleRepo = ruleRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<CategoryRuleDto> Handle(AddCategoryRule request, CancellationToken cancellationToken)
    {
        var rule = new CategoryRule 
        { 
            Keyword = request.Keyword, 
            CategoryId = request.CategoryId,
            FieldType = request.FieldType
        };
        await _ruleRepo.AddAsync(rule);
        
        var category = await _categoryRepo.GetByIdAsync(request.CategoryId);
        return new CategoryRuleDto 
        { 
            Id = rule.Id, 
            Keyword = rule.Keyword, 
            CategoryId = rule.CategoryId,
            FieldType = rule.FieldType,
            Category = category != null ? new CategoryDto(category.Id, category.Name, category.Description) : null
        };
    }

    public async Task<CategoryRuleDto> Handle(UpdateCategoryRule request, CancellationToken cancellationToken)
    {
        var rule = new CategoryRule 
        { 
            Id = request.Id, 
            Keyword = request.Keyword, 
            CategoryId = request.CategoryId,
            FieldType = request.FieldType
        };
        await _ruleRepo.UpdateAsync(rule);
        
        var category = await _categoryRepo.GetByIdAsync(request.CategoryId);
        return new CategoryRuleDto 
        { 
            Id = rule.Id, 
            Keyword = rule.Keyword, 
            CategoryId = rule.CategoryId,
            FieldType = rule.FieldType,
            Category = category != null ? new CategoryDto(category.Id, category.Name, category.Description) : null
        };
    }

    public async Task Handle(DeleteCategoryRule request, CancellationToken cancellationToken)
    {
        await _ruleRepo.DeleteAsync(request.Id);
    }
} 