namespace WSBInvestmentPredictor.Expenses.Shared.Enums;

/// <summary>
/// Defines which transaction field should be checked by category rules
/// </summary>
public enum TransactionFieldType
{
    /// <summary>
    /// Check the transaction title
    /// </summary>
    Title = 1,
    
    /// <summary>
    /// Check the transaction counterparty
    /// </summary>
    Counterparty = 2
} 