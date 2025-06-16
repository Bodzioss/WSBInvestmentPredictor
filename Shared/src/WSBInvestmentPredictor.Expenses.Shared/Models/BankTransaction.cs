namespace WSBInvestmentPredictor.Expenses.Shared.Models;

/// <summary>
/// Represents a bank transaction with all its details.
/// This class is used to store and transfer transaction data between different parts of the application.
/// </summary>
public class BankTransaction
{
    /// <summary>
    /// Gets or sets the date when the transaction was executed.
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the transaction was booked in the account.
    /// </summary>
    public DateTime? BookingDate { get; set; }

    /// <summary>
    /// Gets or sets the name of the counterparty involved in the transaction.
    /// </summary>
    public string? Counterparty { get; set; }

    /// <summary>
    /// Gets or sets the title or description of the transaction.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the account number associated with the transaction.
    /// </summary>
    public string? AccountNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of the bank where the transaction was processed.
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// Gets or sets additional details about the transaction.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets the unique transaction number or identifier.
    /// </summary>
    public string? TransactionNumber { get; set; }

    /// <summary>
    /// Gets or sets the amount of the transaction.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency of the transaction amount.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Gets or sets the amount that was blocked or reserved for the transaction.
    /// </summary>
    public decimal? BlockedAmount { get; set; }

    /// <summary>
    /// Gets or sets the currency of the blocked amount.
    /// </summary>
    public string? BlockedCurrency { get; set; }

    /// <summary>
    /// Gets or sets the payment amount if different from the transaction amount.
    /// </summary>
    public decimal? PaymentAmount { get; set; }

    /// <summary>
    /// Gets or sets the currency of the payment amount.
    /// </summary>
    public string? PaymentCurrency { get; set; }

    /// <summary>
    /// Gets or sets the account identifier where the transaction was processed.
    /// </summary>
    public string? Account { get; set; }

    /// <summary>
    /// Gets or sets the account balance after the transaction was processed.
    /// </summary>
    public decimal? BalanceAfterTransaction { get; set; }

    /// <summary>
    /// Gets or sets the currency of the account balance.
    /// </summary>
    public string? BalanceCurrency { get; set; }
}