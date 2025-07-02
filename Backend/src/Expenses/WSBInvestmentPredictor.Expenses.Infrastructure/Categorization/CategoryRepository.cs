using Microsoft.EntityFrameworkCore;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Infrastructure.Data;

namespace WSBInvestmentPredictor.Expenses.Infrastructure.Categorization;

public class CategoryRepository : ICategoryRepository
{
    private readonly ExpensesDbContext _context;

    public CategoryRepository(ExpensesDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
        => await _context.Categories.ToListAsync();

    public async Task<Category?> GetByIdAsync(int id)
        => await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Category?> GetByNameAsync(string name)
        => await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
} 