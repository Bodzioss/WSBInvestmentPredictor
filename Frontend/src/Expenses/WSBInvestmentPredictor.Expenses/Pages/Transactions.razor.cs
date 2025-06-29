using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using Radzen.Blazor;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Expenses.Models;
using WSBInvestmentPredictor.Expenses.Shared.Dto;

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
    /// Service for displaying dialogs.
    /// </summary>
    [Inject] private Radzen.DialogService DialogService { get; set; } = default!;

    /// <summary>
    /// Service for accessing localized strings.
    /// </summary>
    [Inject] private IStringLocalizer<SharedResource> Loc { get; set; } = default!;

    /// <summary>
    /// Collection of bank transactions to display.
    /// </summary>
    protected IEnumerable<BankTransaction>? transactions;

    /// <summary>
    /// Filtered transactions based on selected category.
    /// </summary>
    protected IEnumerable<BankTransaction>? filteredTransactions;

    /// <summary>
    /// Currently selected category for filtering.
    /// </summary>
    public string? selectedCategory { get; set; }

    /// <summary>
    /// Public property for selected category (for binding in .razor)
    /// </summary>
    public string? SelectedCategory => selectedCategory;

    /// <summary>
    /// Indicates if import dialog is shown.
    /// </summary>
    protected bool showImportDialog;

    /// <summary>
    /// Indicates if edit dialog is shown.
    /// </summary>
    protected bool showEditDialog;

    /// <summary>
    /// Currently selected transaction for editing.
    /// </summary>
    protected BankTransaction? selectedTransaction;

    /// <summary>
    /// Category analysis data for the pie chart.
    /// </summary>
    protected List<CategoryAnalysisDto>? categoryAnalysis;

    /// <summary>
    /// Reference to the data grid component.
    /// </summary>
    protected RadzenDataGrid<BankTransaction>? dataGrid;

    /// <summary>
    /// Initializes the component by loading transaction data.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    /// <summary>
    /// Loads transaction data and category analysis.
    /// </summary>
    protected async Task LoadData()
    {
        try
        {
            // Load transactions
            var query = new GetTransactions();
            var response = await RequestService.Handle<GetTransactions, GetTransactionsResponse>(query);
            transactions = response?.Transactions ?? new List<BankTransaction>();
            
            // Load category analysis
            var analysisQuery = new GetCategoryAnalysis();
            var analysisArray = await RequestService.Handle<GetCategoryAnalysis, CategoryAnalysisDto[]>(analysisQuery);
            categoryAnalysis = analysisArray?.ToList() ?? new List<CategoryAnalysisDto>();
            
            ApplyFilter();
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Error", $"Error loading data: {ex.Message}");
        }
    }

    /// <summary>
    /// Applies category filter to transactions.
    /// </summary>
    private void ApplyFilter()
    {
        if (transactions == null) return;

        if (string.IsNullOrEmpty(selectedCategory))
        {
            filteredTransactions = transactions.ToList();
        }
        else if (selectedCategory == "Uncategorized")
        {
            filteredTransactions = transactions
                .Where(t => string.IsNullOrWhiteSpace(t.Category))
                .ToList();
        }
        else
        {
            filteredTransactions = transactions
                .Where(t => string.Equals(t.Category, selectedCategory, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    /// <summary>
    /// Handles category selection from pie chart.
    /// </summary>
    protected async Task OnCategorySelected(string? category)
    {
        selectedCategory = category;
        ApplyFilter();
        StateHasChanged();
    }

    /// <summary>
    /// Clears the current category filter.
    /// </summary>
    protected void ClearFilter()
    {
        selectedCategory = null;
        ApplyFilter();
        StateHasChanged();
    }

    /// <summary>
    /// Opens import dialog.
    /// </summary>
    protected async Task OpenImportDialog()
    {
        var result = await DialogService.OpenAsync<ImportTransactions>("Import Transactions");
        if (result != null)
        {
            await LoadData();
        }
    }

    /// <summary>
    /// Opens edit dialog for a transaction.
    /// </summary>
    protected async Task EditTransaction(BankTransaction transaction)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Transaction", transaction }
        };

        var result = await DialogService.OpenAsync<EditTransaction>("Edit Transaction", parameters);
        if (result != null)
        {
            await LoadData();
        }
    }
}