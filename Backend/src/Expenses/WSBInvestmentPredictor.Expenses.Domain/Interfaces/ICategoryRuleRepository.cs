using WSBInvestmentPredictor.Expenses.Domain.Categorization;

namespace WSBInvestmentPredictor.Expenses.Domain.Interfaces;

public interface ICategoryRuleRepository
{
    Task<List<CategoryRule>> GetAllAsync();
    Task<CategoryRule?> GetByIdAsync(int id);
    Task AddAsync(CategoryRule rule);
    Task UpdateAsync(CategoryRule rule);
    Task DeleteAsync(int id);
} 