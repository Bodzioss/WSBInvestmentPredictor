namespace WSBInvestmentPredictor.Expenses.Shared.Dto;

public class CategoryAnalysisDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public double Percentage { get; set; }
    public decimal TotalAmount { get; set; }
    public string Color { get; set; } = string.Empty;
} 