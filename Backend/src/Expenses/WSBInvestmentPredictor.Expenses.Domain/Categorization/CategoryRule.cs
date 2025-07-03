using WSBInvestmentPredictor.Expenses.Shared.Enums;

namespace WSBInvestmentPredictor.Expenses.Domain.Categorization;

public class CategoryRule
{
    public int Id { get; set; }
    public string Keyword { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    /// <summary>
    /// Specifies which transaction field should be checked for this rule
    /// </summary>
    public TransactionFieldType FieldType { get; set; } = TransactionFieldType.Title;
} 