using MediatR;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

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