using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Services;

public interface IBankTransactionService
{
    Task<List<BankTransaction>> ProcessCsvFile(Stream fileStream);
}