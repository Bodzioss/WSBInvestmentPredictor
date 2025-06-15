using Microsoft.AspNetCore.Components;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Technology.Cqrs;
using Radzen;

namespace WSBInvestmentPredictor.Expenses.Pages;

public partial class Transactions : ComponentBase
{
    [Inject] private ICqrsRequestService CqrsRequestService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected IEnumerable<BankTransaction>? transactions;
    protected IEnumerable<int>? years;
    protected IEnumerable<string>? filteredAccounts;
    protected IEnumerable<string>? filteredCounterparties;
    protected int selectedYear;
    protected int selectedMonth;
    protected string selectedAccount = string.Empty;
    protected string selectedCounterparty = string.Empty;
    protected decimal totalAmount;
    protected bool isLoading;
    protected string? error;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    protected async Task LoadData()
    {
        try
        {
            isLoading = true;
            error = null;

            var request = new GetTransactions(
                selectedYear > 0 ? selectedYear : null,
                selectedMonth > 0 ? selectedMonth : null,
                !string.IsNullOrEmpty(selectedAccount) ? selectedAccount : null,
                !string.IsNullOrEmpty(selectedCounterparty) ? selectedCounterparty : null
            );

            var result = await CqrsRequestService.Handle<GetTransactions, GetTransactionsResponse>(request);
            transactions = result?.Transactions ?? Enumerable.Empty<BankTransaction>();
            totalAmount = result?.TotalAmount ?? 0;

            filteredAccounts = transactions
                .Select(t => t.Account)
                .Where(a => !string.IsNullOrEmpty(a))
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            filteredCounterparties = transactions
                .Select(t => t.Counterparty)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            years = transactions
                .Select(t => t.TransactionDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();
        }
        catch (Exception ex)
        {
            error = $"Error loading data: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, "Error", error);
            transactions = Enumerable.Empty<BankTransaction>();
            filteredAccounts = Enumerable.Empty<string>();
            filteredCounterparties = Enumerable.Empty<string>();
            years = Enumerable.Empty<int>();
            totalAmount = 0;
        }
        finally
        {
            isLoading = false;
        }
    }

    protected async Task OnYearChanged()
    {
        if (selectedYear == 0)
        {
            selectedMonth = 0;
        }
        await LoadData();
    }

    protected async Task OnMonthChanged()
    {
        await LoadData();
    }

    protected async Task OnAccountChanged()
    {
        await LoadData();
    }

    protected async Task OnCounterpartyChanged()
    {
        await LoadData();
    }
} 