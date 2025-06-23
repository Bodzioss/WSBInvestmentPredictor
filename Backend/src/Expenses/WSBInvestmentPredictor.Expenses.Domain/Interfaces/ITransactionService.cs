using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Domain.Interfaces;

public interface ITransactionService
{
    Task<GetTransactionsResponse> Handle(GetTransactions query);

    Task Handle(AddTransactions command);

    Task<List<BankTransaction>> ProcessCsvFile(Stream fileStream);
}