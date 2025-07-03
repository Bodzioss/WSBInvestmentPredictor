using MediatR;
using System.Linq;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using WSBInvestmentPredictor.Expenses.Shared.Enums;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Application.Categorization;

public class CategoryCommandHandler :
    IRequestHandler<AddCategory, CategoryDto>,
    IRequestHandler<UpdateCategory, CategoryDto>,
    IRequestHandler<DeleteCategory>,
    IRequestHandler<AssignCategoryToTransaction>,
    IRequestHandler<ApplyCategoryRules>
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly ICategoryRuleRepository _ruleRepo;
    private readonly ITransactionRepository _transactionRepo;
    
    public CategoryCommandHandler(
        ICategoryRepository categoryRepo, 
        ICategoryRuleRepository ruleRepo, 
        ITransactionRepository transactionRepo)
    {
        _categoryRepo = categoryRepo;
        _ruleRepo = ruleRepo;
        _transactionRepo = transactionRepo;
    }

    public async Task<CategoryDto> Handle(AddCategory request, CancellationToken cancellationToken)
    {
        // Check if category already exists
        var existingCategory = await _categoryRepo.GetByNameAsync(request.Name);
        
        if (existingCategory != null)
        {
            return new CategoryDto(existingCategory.Id, existingCategory.Name, existingCategory.Description);
        }

        // Add new category
        var category = new Category { Name = request.Name, Description = request.Description };
        await _categoryRepo.AddAsync(category);
        
        return new CategoryDto(category.Id, category.Name, category.Description);
    }

    public async Task<CategoryDto> Handle(UpdateCategory request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepo.GetByIdAsync(request.Id);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {request.Id} does not exist");

        category.Name = request.Name;
        category.Description = request.Description;
        await _categoryRepo.UpdateAsync(category);
        
        return new CategoryDto(category.Id, category.Name, category.Description);
    }

    public async Task Handle(DeleteCategory request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[DeleteCategory] ID: {request.Id}");
            
            // Delete all rules for this category
            var rules = await _ruleRepo.GetAllAsync();
            foreach (var rule in rules.Where(r => r.CategoryId == request.Id).ToList())
            {
                Console.WriteLine($"[DeleteCategory] Deleting rule.Id={rule.Id}, CategoryId={rule.CategoryId}");
                await _ruleRepo.DeleteAsync(rule.Id);
            }
            
            // Delete the category
            await _categoryRepo.DeleteAsync(request.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DeleteCategory] Exception: {ex}");
            throw;
        }
    }

    public async Task Handle(AssignCategoryToTransaction request, CancellationToken cancellationToken)
    {
        var all = await _transactionRepo.GetAllAsync();
        var tx = all.FirstOrDefault(t => t.Id == request.TransactionId);
        if (tx != null)
        {
            var category = await _categoryRepo.GetByIdAsync(request.CategoryId);
            var categoryName = category?.Name ?? string.Empty;
            
            // Directly modify the property instead of creating new instance
            tx.Category = categoryName;
            await _transactionRepo.UpdateAsync(tx);
        }
    }

    public async Task Handle(ApplyCategoryRules request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("[ApplyCategoryRules] Starting transaction categorization by rules");
            
            var rules = await _ruleRepo.GetAllAsync();
            var categories = await _categoryRepo.GetAllAsync();
            var transactions = (await _transactionRepo.GetAllAsync()).ToList();

            Console.WriteLine($"[ApplyCategoryRules] Found {rules.Count} rules and {transactions.Count} transactions");

            int categorizedCount = 0;
            foreach (var rule in rules)
            {
                if (string.IsNullOrWhiteSpace(rule.Keyword))
                    continue;

                var category = categories.FirstOrDefault(c => c.Id == rule.CategoryId);
                if (category == null)
                    continue;

                // Apply rules to all transactions that match the rule, regardless of current category
                var matchingTransactions = transactions.Where(t => 
                    IsTransactionMatchingRule(t, rule)).ToList();

                foreach (var tx in matchingTransactions)
                {
                    Console.WriteLine($"[ApplyCategoryRules] Categorizing transaction '{tx.Title}' -> '{category.Name}' (rule: {rule.Keyword} on field {rule.FieldType})");
                    
                    // Directly modify the property instead of creating new instance
                    tx.Category = category.Name;
                    await _transactionRepo.UpdateAsync(tx);
                    categorizedCount++;
                }
            }

            Console.WriteLine($"[ApplyCategoryRules] Categorization completed. Categorized {categorizedCount} transactions");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApplyCategoryRules] Error during categorization: {ex}");
            throw;
        }
    }

    private static bool IsTransactionMatchingRule(BankTransaction transaction, CategoryRule rule)
    {
        var fieldToCheck = rule.FieldType switch
        {
            TransactionFieldType.Title => transaction.Title,
            TransactionFieldType.Counterparty => transaction.Counterparty,
            _ => transaction.Title // Default to title for unknown field types
        };

        return !string.IsNullOrEmpty(fieldToCheck) && 
               fieldToCheck.Contains(rule.Keyword, StringComparison.OrdinalIgnoreCase);
    }
} 