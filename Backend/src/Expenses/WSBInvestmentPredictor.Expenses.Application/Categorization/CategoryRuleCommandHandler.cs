using MediatR;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using WSBInvestmentPredictor.Expenses.Shared.Enums;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Application.Categorization;

public class CategoryRuleCommandHandler :
    IRequestHandler<AddCategoryRule, CategoryRuleDto>,
    IRequestHandler<UpdateCategoryRule, CategoryRuleDto>,
    IRequestHandler<DeleteCategoryRule>
{
    private readonly ICategoryRuleRepository _ruleRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly ITransactionRepository _transactionRepo;
    
    public CategoryRuleCommandHandler(
        ICategoryRuleRepository ruleRepo, 
        ICategoryRepository categoryRepo,
        ITransactionRepository transactionRepo)
    {
        _ruleRepo = ruleRepo;
        _categoryRepo = categoryRepo;
        _transactionRepo = transactionRepo;
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
        
        // Apply rules automatically after adding a new rule
        await ApplyRulesToTransactions();
        
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
        
        // Apply rules automatically after updating a rule
        await ApplyRulesToTransactions();
        
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
        
        // Apply rules automatically after deleting a rule
        await ApplyRulesToTransactions();
    }

    private async Task ApplyRulesToTransactions()
    {
        try
        {
            Console.WriteLine("[CategoryRuleCommandHandler] Automatically applying rules after rule change");
            
            var rules = await _ruleRepo.GetAllAsync();
            var categories = await _categoryRepo.GetAllAsync();
            var transactions = (await _transactionRepo.GetAllAsync()).ToList();

            Console.WriteLine($"[CategoryRuleCommandHandler] Found {rules.Count} rules and {transactions.Count} transactions");

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
                    Console.WriteLine($"[CategoryRuleCommandHandler] Auto-categorizing transaction '{tx.Title}' -> '{category.Name}' (rule: {rule.Keyword} on field {rule.FieldType})");
                    
                    // Directly modify the property instead of creating new instance
                    tx.Category = category.Name;
                    await _transactionRepo.UpdateAsync(tx);
                    categorizedCount++;
                }
            }

            Console.WriteLine($"[CategoryRuleCommandHandler] Auto-categorization completed. Categorized {categorizedCount} transactions");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CategoryRuleCommandHandler] Error during auto-categorization: {ex}");
            // Don't throw the exception to avoid breaking the main operation
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