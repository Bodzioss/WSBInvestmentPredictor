using WSBInvestmentPredictor.Expenses.Domain.Categorization;

namespace WSBInvestmentPredictor.Expenses.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetByNameAsync(string name);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
} 