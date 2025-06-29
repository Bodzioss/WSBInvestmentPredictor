using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;

namespace WSBInvestmentPredictor.Expenses.Infrastructure.Categorization;

public class CategoryRuleRepository : ICategoryRuleRepository
{
    private static readonly List<CategoryRule> _rules = new();
    private static int _nextId = 1;

    public Task<List<CategoryRule>> GetAllAsync()
        => Task.FromResult(_rules.ToList());

    public Task<CategoryRule?> GetByIdAsync(int id)
        => Task.FromResult(_rules.FirstOrDefault(r => r.Id == id));

    public Task AddAsync(CategoryRule rule)
    {
        rule.Id = _nextId++;
        _rules.Add(rule);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CategoryRule rule)
    {
        var existing = _rules.FirstOrDefault(r => r.Id == rule.Id);
        if (existing != null)
        {
            existing.Keyword = rule.Keyword;
            existing.CategoryId = rule.CategoryId;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var rule = _rules.FirstOrDefault(r => r.Id == id);
        if (rule != null)
            _rules.Remove(rule);
        return Task.CompletedTask;
    }
} 