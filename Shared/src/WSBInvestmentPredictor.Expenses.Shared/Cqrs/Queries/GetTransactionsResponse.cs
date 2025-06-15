using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;

public record GetTransactionsResponse(
    IEnumerable<BankTransaction> Transactions,
    decimal TotalAmount
); 