using MediatR;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Expenses.Domain.Categorization;
using WSBInvestmentPredictor.Expenses.Shared.Enums;

namespace WSBInvestmentPredictor.Expenses.Application.Commands;

public class AddTransactionsHandler : IRequestHandler<AddTransactions>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRuleRepository _ruleRepository;
    private readonly ICategoryRepository _categoryRepository;

    public AddTransactionsHandler(
        ITransactionRepository transactionRepository,
        ICategoryRuleRepository ruleRepository,
        ICategoryRepository categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _ruleRepository = ruleRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(AddTransactions request, CancellationToken cancellationToken)
    {
        await _transactionRepository.AddTransactions(request.Transactions);
        
        // Apply rules automatically after adding transactions
        await ApplyRulesToNewTransactions(request.Transactions);
    }

    private async Task ApplyRulesToNewTransactions(IEnumerable<BankTransaction> newTransactions)
    {
        try
        {
            Console.WriteLine("[AddTransactionsHandler] Automatically applying rules to new transactions");
            
            var rules = await _ruleRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            Console.WriteLine($"[AddTransactionsHandler] Found {rules.Count} rules for {newTransactions.Count()} new transactions");

            int categorizedCount = 0;
            foreach (var transaction in newTransactions)
            {
                foreach (var rule in rules)
                {
                    if (string.IsNullOrWhiteSpace(rule.Keyword))
                        continue;

                    var category = categories.FirstOrDefault(c => c.Id == rule.CategoryId);
                    if (category == null)
                        continue;

                    if (IsTransactionMatchingRule(transaction, rule))
                    {
                        Console.WriteLine($"[AddTransactionsHandler] Auto-categorizing new transaction '{transaction.Title}' -> '{category.Name}' (rule: {rule.Keyword} on field {rule.FieldType})");
                        
                        // Update the transaction with the category
                        transaction.Category = category.Name;
                        await _transactionRepository.UpdateAsync(transaction);
                        categorizedCount++;
                        break; // Stop after first match to avoid overwriting with multiple rules
                    }
                }
            }

            Console.WriteLine($"[AddTransactionsHandler] Auto-categorization completed. Categorized {categorizedCount} new transactions");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AddTransactionsHandler] Error during auto-categorization: {ex}");
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

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand>
{
    private readonly ITransactionRepository _transactionRepository;

    public UpdateTransactionCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new BankTransaction(
            request.Date,  // TransactionDate
            request.Title,
            request.Amount,
            request.Account,
            request.Counterparty)
        {
            Id = request.Id,
            Category = request.Category,
            Currency = request.Currency
        };

        await _transactionRepository.UpdateAsync(transaction);
    }
}

public class ClearAllTransactionsHandler : IRequestHandler<ClearAllTransactions>
{
    private readonly ITransactionRepository _transactionRepository;

    public ClearAllTransactionsHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task Handle(ClearAllTransactions request, CancellationToken cancellationToken)
    {
        await _transactionRepository.Clear();
    }
}