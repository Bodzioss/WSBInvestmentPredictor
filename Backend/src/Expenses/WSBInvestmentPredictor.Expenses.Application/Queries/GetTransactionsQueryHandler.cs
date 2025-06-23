using MediatR;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;

namespace WSBInvestmentPredictor.Expenses.Application.Queries;

public class GetTransactionsHandler : IRequestHandler<GetTransactions, GetTransactionsResponse>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<GetTransactionsResponse> Handle(GetTransactions request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetTransactions(
            request.Year,
            request.Month,
            request.Account,
            request.Counterparty);

        var totalAmount = transactions.Sum(t => t.Amount);

        return new GetTransactionsResponse(transactions, totalAmount);
    }
}