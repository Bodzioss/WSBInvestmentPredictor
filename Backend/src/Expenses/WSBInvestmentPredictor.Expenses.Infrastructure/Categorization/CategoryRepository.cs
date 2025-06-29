using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;

namespace WSBInvestmentPredictor.Expenses.Infrastructure.Categorization;

public class CategoryRepository : ICategoryRepository
{
    private static readonly List<Category> _categories = new();
    private static int _nextId = 1;

    public Task<List<Category>> GetAllAsync()
        => Task.FromResult(_categories.ToList());

    public Task<Category?> GetByIdAsync(int id)
        => Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));

    public Task<Category?> GetByNameAsync(string name)
        => Task.FromResult(_categories.FirstOrDefault(c => c.Name == name));

    public Task AddAsync(Category category)
    {
        category.Id = _nextId++;
        _categories.Add(category);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Category category)
    {
        var existing = _categories.FirstOrDefault(c => c.Id == category.Id);
        if (existing != null)
        {
            existing.Name = category.Name;
            existing.Description = category.Description;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category != null)
            _categories.Remove(category);
        return Task.CompletedTask;
    }
} 