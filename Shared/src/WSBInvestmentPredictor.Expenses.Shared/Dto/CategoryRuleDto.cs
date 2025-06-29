using WSBInvestmentPredictor.Expenses.Shared.Enums;

namespace WSBInvestmentPredictor.Expenses.Shared.Dto;

/// <summary>
/// Data Transfer Object representing a categorization rule for transactions.
/// </summary>
public class CategoryRuleDto
{
    /// <summary>
    /// Unique identifier of the rule.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Keyword that triggers the rule.
    /// </summary>
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// Category ID assigned by the rule.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Category assigned by the rule.
    /// </summary>
    public CategoryDto? Category { get; set; }
    
    /// <summary>
    /// Specifies which transaction field should be checked for this rule.
    /// </summary>
    public TransactionFieldType FieldType { get; set; } = TransactionFieldType.Title;
} 