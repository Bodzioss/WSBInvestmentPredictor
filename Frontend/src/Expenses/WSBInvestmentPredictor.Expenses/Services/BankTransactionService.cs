using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using WSBInvestmentPredictor.Expenses.Services.Converters;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Services;

public class BankTransactionService : IBankTransactionService
{
    static BankTransactionService()
    {
        // Register encoding provider for Windows-1250
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<List<BankTransaction>> ProcessCsvFile(Stream fileStream)
    {
        var config = new CsvConfiguration(new CultureInfo("pl-PL"))
        {
            Delimiter = ";",
            Encoding = Encoding.GetEncoding(1250), // Use Windows-1250 encoding for Polish characters
            HasHeaderRecord = false, // Disable automatic header handling
            MissingFieldFound = null
        };

        using var reader = new StreamReader(fileStream, Encoding.GetEncoding(1250));
        using var csv = new CsvReader(reader, config);

        // Register type converters
        csv.Context.TypeConverterCache.AddConverter<DateTime>(new UniversalDateTimeConverter());
        csv.Context.TypeConverterCache.AddConverter<DateTime?>(new UniversalDateTimeConverter());
        csv.Context.TypeConverterCache.AddConverter<decimal>(new UniversalDecimalConverter());
        csv.Context.TypeConverterCache.AddConverter<decimal?>(new UniversalDecimalConverter());

        var transactions = new List<BankTransaction>();
        
        // Skip the header rows (first 15 lines in the example)
        for (int i = 0; i < 15; i++)
        {
            await csv.ReadAsync();
        }

        while (await csv.ReadAsync())
        {
            try
            {
                // Log the entire row for debugging
                var rowData = new List<string>();
                for (int i = 0; i < 17; i++)
                {
                    rowData.Add(csv.GetField(i));
                }
                Console.WriteLine($"Raw CSV row: {string.Join(" | ", rowData)}");

                var transaction = new BankTransaction
                {
                    TransactionDate = csv.GetField<DateTime>(0),
                    BookingDate = csv.GetField<DateTime?>(1),
                    Counterparty = csv.GetField<string>(2),
                    Title = csv.GetField<string>(3),
                    AccountNumber = csv.GetField<string>(4),
                    BankName = csv.GetField<string>(5),
                    Details = csv.GetField<string>(6),
                    TransactionNumber = csv.GetField<string>(7),
                    Amount = csv.GetField<decimal>(8),
                    Currency = csv.GetField<string>(9),
                    BlockedAmount = csv.GetField<decimal?>(10),
                    BlockedCurrency = csv.GetField<string>(11),
                    PaymentAmount = csv.GetField<decimal?>(12),
                    PaymentCurrency = csv.GetField<string>(13),
                    Account = csv.GetField<string>(14),
                    BalanceAfterTransaction = csv.GetField<decimal?>(15),
                    BalanceCurrency = csv.GetField<string>(16)
                };

                transactions.Add(transaction);
            }
            catch (Exception ex)
            {
                // Log error and continue with next record
                Console.WriteLine($"Error processing row: {ex.Message}");
            }
        }

        return transactions;
    }
} 