using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Domain.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<GetTransactionsResponse> Handle(GetTransactions query)
    {
        var transactions = await _transactionRepository.GetTransactions(
            query.Year,
            query.Month,
            query.Account,
            query.Counterparty);

        var totalAmount = transactions.Sum(t => t.Amount);

        return new GetTransactionsResponse(transactions, totalAmount);
    }

    public async Task Handle(AddTransactions command)
    {
        await _transactionRepository.AddTransactions(command.Transactions);
    }

    public async Task<List<BankTransaction>> ProcessCsvFile(Stream fileStream)
    {
        var config = new CsvConfiguration(new CultureInfo("pl-PL"))
        {
            Delimiter = ";",
            Encoding = Encoding.GetEncoding(1250),
            HasHeaderRecord = false,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(fileStream, Encoding.GetEncoding(1250));
        using var csv = new CsvReader(reader, config);

        var transactions = new List<BankTransaction>();

        // Skip the header rows
        for (int i = 0; i < 15; i++)
        {
            await csv.ReadAsync();
        }

        while (await csv.ReadAsync())
        {
            var transaction = new BankTransaction(
                csv.GetField<DateTime>(0),
                csv.GetField<string>(3),
                csv.GetField<decimal>(8),
                csv.GetField<string>(14),
                csv.GetField<string>(2)
            )
            {
                BookingDate = csv.GetField<DateTime?>(1),
                AccountNumber = csv.GetField<string>(4),
                BankName = csv.GetField<string>(5),
                Details = csv.GetField<string>(6),
                TransactionNumber = csv.GetField<string>(7),
                Currency = csv.GetField<string>(9),
                BlockedAmount = csv.GetField<decimal?>(10),
                BlockedCurrency = csv.GetField<string>(11),
                PaymentAmount = csv.GetField<decimal?>(12),
                PaymentCurrency = csv.GetField<string>(13),
                BalanceAfterTransaction = csv.GetField<decimal?>(15),
                BalanceCurrency = csv.GetField<string>(16)
            };

            transactions.Add(transaction);
        }

        return transactions;
    }
}