using Microsoft.EntityFrameworkCore;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Infrastructure.Data;

namespace WSBInvestmentPredictor.Expenses.Infrastructure.Categorization;

public class CategoryRuleRepository : ICategoryRuleRepository
{
    private readonly ExpensesDbContext _context;

    public CategoryRuleRepository(ExpensesDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryRule>> GetAllAsync()
        => await _context.CategoryRules.Include(r => r.Category).ToListAsync();

    public async Task<CategoryRule?> GetByIdAsync(int id)
        => await _context.CategoryRules.Include(r => r.Category).FirstOrDefaultAsync(r => r.Id == id);

    public async Task AddAsync(CategoryRule rule)
    {
        await _context.CategoryRules.AddAsync(rule);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CategoryRule rule)
    {
        _context.CategoryRules.Update(rule);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var rule = await _context.CategoryRules.FirstOrDefaultAsync(r => r.Id == id);
        if (rule != null)
        {
            _context.CategoryRules.Remove(rule);
            await _context.SaveChangesAsync();
        }
    }
} 