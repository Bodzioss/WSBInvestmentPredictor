using System;

namespace WSBInvestmentPredictor.Expenses.Shared.Models;

public class BankTransaction
{
    public DateTime TransactionDate { get; set; }
    public DateTime? BookingDate { get; set; }
    public string? Counterparty { get; set; }
    public string? Title { get; set; }
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public string? Details { get; set; }
    public string? TransactionNumber { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public decimal? BlockedAmount { get; set; }
    public string? BlockedCurrency { get; set; }
    public decimal? PaymentAmount { get; set; }
    public string? PaymentCurrency { get; set; }
    public string? Account { get; set; }
    public decimal? BalanceAfterTransaction { get; set; }
    public string? BalanceCurrency { get; set; }
} 