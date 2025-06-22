using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Frontend.Shared.Resources;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Pages;

/// <summary>
/// Component for displaying and filtering bank transactions.
/// Provides functionality to view transactions with various filters and aggregations.
/// </summary>
public partial class Transactions : ComponentBase
{
    /// <summary>
    /// Service for handling CQRS requests to the backend.
    /// </summary>
    [Inject] private ICqrsRequestService RequestService { get; set; } = default!;

    /// <summary>
    /// Service for displaying notifications to the user.
    /// </summary>
    [Inject] private Radzen.NotificationService NotificationService { get; set; } = default!;

    /// <summary>
    /// Service for accessing localized strings.
    /// </summary>
    [Inject] private IStringLocalizer<SharedResource> Loc { get; set; } = default!;

    /// <summary>
    /// Collection of bank transactions to display.
    /// </summary>
    protected IEnumerable<BankTransaction>? transactions;

    /// <summary>
    /// Available years for filtering transactions.
    /// </summary>
    protected IEnumerable<int>? years;

    /// <summary>
    /// Available accounts for filtering transactions.
    /// </summary>
    protected IEnumerable<string>? filteredAccounts;

    /// <summary>
    /// Available counterparties for filtering transactions.
    /// </summary>
    protected IEnumerable<string>? filteredCounterparties;

    /// <summary>
    /// Currently selected year filter.
    /// </summary>
    protected int selectedYear;

    /// <summary>
    /// Currently selected month filter.
    /// </summary>
    protected int selectedMonth;

    /// <summary>
    /// Currently selected account filter.
    /// </summary>
    protected string selectedAccount = string.Empty;

    /// <summary>
    /// Currently selected counterparty filter.
    /// </summary>
    protected string selectedCounterparty = string.Empty;

    /// <summary>
    /// Total amount of filtered transactions.
    /// </summary>
    protected decimal totalAmount;

    /// <summary>
    /// Indicates if data is currently being loaded.
    /// </summary>
    protected bool isLoading;

    /// <summary>
    /// Error message if data loading fails.
    /// </summary>
    protected string? error;

    /// <summary>
    /// Initializes the component by loading transaction data.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    /// <summary>
    /// Loads transaction data based on current filter selections.
    /// Updates available filters and total amount.
    /// </summary>
    protected async Task LoadData()
    {
        try
        {
            isLoading = true;
            error = null;

            // Create query with current filter values
            var request = new GetTransactions(
                selectedYear > 0 ? selectedYear : null,
                selectedMonth > 0 ? selectedMonth : null,
                !string.IsNullOrEmpty(selectedAccount) ? selectedAccount : null,
                !string.IsNullOrEmpty(selectedCounterparty) ? selectedCounterparty : null
            );

            // Fetch transactions from the backend
            var result = await RequestService.Handle<GetTransactions, GetTransactionsResponse>(request);
            transactions = result?.Transactions ?? Enumerable.Empty<BankTransaction>();
            totalAmount = result?.TotalAmount ?? 0;

            // Update available filter options based on loaded data
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
            // Handle errors and reset data
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

    /// <summary>
    /// Handles year filter change.
    /// Resets month selection if year is cleared.
    /// </summary>
    protected async Task OnYearChanged()
    {
        if (selectedYear == 0)
        {
            selectedMonth = 0;
        }
        await LoadData();
    }

    /// <summary>
    /// Handles month filter change.
    /// </summary>
    protected async Task OnMonthChanged()
    {
        await LoadData();
    }

    /// <summary>
    /// Handles account filter change.
    /// </summary>
    protected async Task OnAccountChanged()
    {
        await LoadData();
    }

    /// <summary>
    /// Handles counterparty filter change.
    /// </summary>
    protected async Task OnCounterpartyChanged()
    {
        await LoadData();
    }
}