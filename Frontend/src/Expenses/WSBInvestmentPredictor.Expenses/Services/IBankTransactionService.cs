using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Services;

/// <summary>
/// Service interface for processing bank transaction data from CSV files.
/// Provides functionality to parse and convert CSV data into bank transaction objects.
/// </summary>
public interface IBankTransactionService
{
    /// <summary>
    /// Processes a CSV file stream and converts it into a list of bank transactions.
    /// </summary>
    /// <param name="fileStream">The stream containing the CSV file data</param>
    /// <returns>A list of parsed bank transactions</returns>
    Task<List<BankTransaction>> ProcessCsvFile(Stream fileStream);
}