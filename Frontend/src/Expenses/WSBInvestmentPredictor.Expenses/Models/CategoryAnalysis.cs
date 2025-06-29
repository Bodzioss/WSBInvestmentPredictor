namespace WSBInvestmentPredictor.Expenses.Models;

/// <summary>
/// Represents category analysis data for visualization
/// </summary>
public class CategoryAnalysis
{
    /// <summary>
    /// Category name or "Uncategorized" for transactions without category
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Number of transactions in this category
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// Percentage of total transactions
    /// </summary>
    public double Percentage { get; set; }

    /// <summary>
    /// Total amount for this category
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Color for the pie chart segment
    /// </summary>
    public string Color { get; set; } = string.Empty;
} 