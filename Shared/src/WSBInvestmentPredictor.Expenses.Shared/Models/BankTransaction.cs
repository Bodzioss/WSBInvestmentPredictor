namespace WSBInvestmentPredictor.Expenses.Shared.Models;

/// <summary>
/// Represents a bank transaction with all its details.
/// This class is used to store and transfer transaction data between different parts of the application.
/// </summary>
public record BankTransaction(DateTime TransactionDate, string? Title, decimal Amount, string? Account, string? Counterparty)
{
    public int Id { get; set; }
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the date when the transaction was booked in the account.
    /// </summary>
    public DateTime? BookingDate { get; init; }

    /// <summary>
    /// Gets or sets the account number associated with the transaction.
    /// </summary>
    public string? AccountNumber { get; init; }

    /// <summary>
    /// Gets or sets the name of the bank where the transaction was processed.
    /// </summary>
    public string? BankName { get; init; }

    /// <summary>
    /// Gets or sets additional details about the transaction.
    /// </summary>
    public string? Details { get; init; }

    /// <summary>
    /// Gets or sets the unique transaction number or identifier.
    /// </summary>
    public string? TransactionNumber { get; init; }

    /// <summary>
    /// Gets or sets the currency of the transaction amount.
    /// </summary>
    public string? Currency { get; init; }

    /// <summary>
    /// Gets or sets the amount that was blocked or reserved for the transaction.
    /// </summary>
    public decimal? BlockedAmount { get; init; }

    /// <summary>
    /// Gets or sets the currency of the blocked amount.
    /// </summary>
    public string? BlockedCurrency { get; init; }

    /// <summary>
    /// Gets or sets the payment amount if different from the transaction amount.
    /// </summary>
    public decimal? PaymentAmount { get; init; }

    /// <summary>
    /// Gets or sets the currency of the payment amount.
    /// </summary>
    public string? PaymentCurrency { get; init; }

    /// <summary>
    /// Gets or sets the account balance after the transaction was processed.
    /// </summary>
    public decimal? BalanceAfterTransaction { get; init; }

    /// <summary>
    /// Gets or sets the currency of the account balance.
    /// </summary>
    public string? BalanceCurrency { get; init; }
}