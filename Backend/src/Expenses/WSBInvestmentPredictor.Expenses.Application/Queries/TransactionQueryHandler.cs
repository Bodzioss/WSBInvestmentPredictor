using MediatR;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Expenses.Shared.Dto;

namespace WSBInvestmentPredictor.Expenses.Application.Queries;

public class TransactionQueryHandler :
    IRequestHandler<GetTransactions, GetTransactionsResponse>,
    IRequestHandler<GetUncategorizedTransactions, List<BankTransaction>>,
    IRequestHandler<GetCategoryAnalysis, List<CategoryAnalysisDto>>
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionQueryHandler(ITransactionRepository transactionRepository)
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

    public async Task<List<BankTransaction>> Handle(GetUncategorizedTransactions request, CancellationToken cancellationToken)
    {
        var all = await _transactionRepository.GetTransactions();
        return all.Where(t => string.IsNullOrWhiteSpace(t.Category)).ToList();
    }

    public async Task<List<CategoryAnalysisDto>> Handle(GetCategoryAnalysis request, CancellationToken cancellationToken)
    {
        var allTransactions = await _transactionRepository.GetAllAsync();
        if (allTransactions == null || !allTransactions.Any())
            return new List<CategoryAnalysisDto>();

        // Filter only outflows (negative amounts)
        var outflows = allTransactions.Where(t => t.Amount < 0).ToList();
        if (!outflows.Any())
            return new List<CategoryAnalysisDto>();

        var totalCount = outflows.Count;
        var grouped = outflows
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Category) ? "Uncategorized" : t.Category!)
            .Select((g, idx) => new CategoryAnalysisDto
            {
                CategoryName = g.Key,
                TransactionCount = g.Count(),
                Percentage = (double)g.Count() / totalCount * 100,
                TotalAmount = Math.Abs(g.Sum(t => t.Amount)), // Use absolute value for display
                Color = GetColor(idx)
            })
            .OrderByDescending(c => c.TotalAmount) // Sort by TotalAmount
            .ToList();
        return grouped;
    }

    private static string GetColor(int idx)
    {
        var colors = new[]
        {
            "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF",
            "#FF9F40", "#FF6384", "#C9CBCF", "#4BC0C0", "#FF6384"
        };
        return colors[idx % colors.Length];
    }
} 