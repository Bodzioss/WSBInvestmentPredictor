using MediatR;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Application.Commands;

public class AddTransactionsHandler : IRequestHandler<AddTransactions>
{
    private readonly ITransactionRepository _transactionRepository;

    public AddTransactionsHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task Handle(AddTransactions request, CancellationToken cancellationToken)
    {
        await _transactionRepository.AddTransactions(request.Transactions);
    }
}

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand>
{
    private readonly ITransactionRepository _transactionRepository;

    public UpdateTransactionCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new BankTransaction(
            request.Date,  // TransactionDate
            request.Title,
            request.Amount,
            request.Account,
            request.Counterparty)
        {
            Id = request.Id,
            Category = request.Category,
            Currency = request.Currency
        };

        await _transactionRepository.UpdateAsync(transaction);
    }
}