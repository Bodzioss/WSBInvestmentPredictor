using WSBInvestmentPredictor.Expenses.Models;

namespace WSBInvestmentPredictor.Expenses.Services;

public interface IBankTransactionService
{
    Task<List<BankTransaction>> ProcessCsvFile(Stream fileStream);
} 