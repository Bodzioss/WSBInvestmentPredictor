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
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;

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
        var result = await DialogService.OpenAsync<ImportTransactionsDialog>("Import Transactions");
        if (result != null && (bool)result)
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

    /// <summary>
    /// Shows the help modal.
    /// </summary>
    protected async Task ShowHelp()
    {
        await DialogService.OpenAsync<TransactionsHelp>("Pomoc - Transakcje");
    }

    /// <summary>
    /// Adds sample transaction data for demonstration purposes.
    /// </summary>
    protected async Task AddSampleData()
    {
        try
        {
            // First, create sample categories if they don't exist
            await CreateSampleCategories();
            
            // Then add sample transactions
            var sampleTransactions = GenerateSampleTransactions();
            var command = new AddTransactions(sampleTransactions);
            await RequestService.Handle(command);
            
            NotificationService.Notify(NotificationSeverity.Success, "Success", "Sample data added successfully!");
            await LoadData();
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Error", $"Error adding sample data: {ex.Message}");
        }
    }

    /// <summary>
    /// Clears all transactions from the system after user confirmation.
    /// </summary>
    protected async Task ClearAllTransactions()
    {
        var confirmed = await DialogService.Confirm(
            "Are you sure you want to delete ALL transactions? This action cannot be undone.",
            "Clear All Transactions",
            new ConfirmOptions
            {
                OkButtonText = "Yes, Delete All",
                CancelButtonText = "Cancel"
            });

        if (confirmed == true)
        {
            try
            {
                var command = new ClearAllTransactions();
                await RequestService.Handle(command);
                
                NotificationService.Notify(NotificationSeverity.Success, "Success", "All transactions have been cleared!");
                await LoadData();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", $"Error clearing transactions: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Creates sample categories for demonstration purposes.
    /// </summary>
    private async Task CreateSampleCategories()
    {
        var sampleCategories = new[]
        {
            new { Name = "Groceries", Description = "Food and household items" },
            new { Name = "Transportation", Description = "Fuel, public transport, and travel expenses" },
            new { Name = "Entertainment", Description = "Movies, games, and leisure activities" },
            new { Name = "Restaurants", Description = "Dining out and food delivery" },
            new { Name = "Shopping", Description = "Online and offline shopping" },
            new { Name = "Subscriptions", Description = "Monthly subscriptions and services" }
        };

        foreach (var category in sampleCategories)
        {
            try
            {
                await RequestService.Handle<AddCategory, CategoryDto>(new AddCategory(category.Name, category.Description));
            }
            catch
            {
                // Category might already exist, ignore the error
            }
        }
    }

    /// <summary>
    /// Generates a collection of sample transactions for demonstration.
    /// </summary>
    /// <returns>Collection of sample bank transactions</returns>
    private List<BankTransaction> GenerateSampleTransactions()
    {
        var random = new Random();
        var transactions = new List<BankTransaction>();
        var baseDate = DateTime.Now.AddDays(-30); // Start from 30 days ago

        // Sample categories and their keywords
        var categorizedTransactions = new[]
        {
            // Groceries - 5 transactions
            new { Title = "Biedronka", Counterparty = "Biedronka", Amount = -45.67m, Category = "Groceries" },
            new { Title = "Lidl", Counterparty = "Lidl", Amount = -32.89m, Category = "Groceries" },
            new { Title = "Carrefour", Counterparty = "Carrefour", Amount = -78.45m, Category = "Groceries" },
            new { Title = "Żabka", Counterparty = "Żabka", Amount = -12.30m, Category = "Groceries" },
            new { Title = "Tesco", Counterparty = "Tesco", Amount = -56.78m, Category = "Groceries" },
            
            // Transportation - 5 transactions
            new { Title = "Orlen", Counterparty = "Orlen", Amount = -120.00m, Category = "Transportation" },
            new { Title = "BP", Counterparty = "BP", Amount = -95.50m, Category = "Transportation" },
            new { Title = "Shell", Counterparty = "Shell", Amount = -110.25m, Category = "Transportation" },
            new { Title = "PKP", Counterparty = "PKP", Amount = -45.00m, Category = "Transportation" },
            new { Title = "MPK", Counterparty = "MPK", Amount = -15.00m, Category = "Transportation" }
        };

        // Uncategorized transactions - 10 transactions
        var uncategorizedTransactions = new[]
        {
            new { Title = "Netflix", Counterparty = "Netflix", Amount = -29.99m, Category = (string?)null },
            new { Title = "Spotify", Counterparty = "Spotify", Amount = -19.99m, Category = (string?)null },
            new { Title = "Amazon", Counterparty = "Amazon", Amount = -89.99m, Category = (string?)null },
            new { Title = "Allegro", Counterparty = "Allegro", Amount = -156.78m, Category = (string?)null },
            new { Title = "Empik", Counterparty = "Empik", Amount = -67.45m, Category = (string?)null },
            new { Title = "Cinema City", Counterparty = "Cinema City", Amount = -35.00m, Category = (string?)null },
            new { Title = "McDonald's", Counterparty = "McDonald's", Amount = -28.50m, Category = (string?)null },
            new { Title = "KFC", Counterparty = "KFC", Amount = -42.30m, Category = (string?)null },
            new { Title = "Pizza Hut", Counterparty = "Pizza Hut", Amount = -65.00m, Category = (string?)null },
            new { Title = "Starbucks", Counterparty = "Starbucks", Amount = -18.75m, Category = (string?)null }
        };

        // Add categorized transactions
        for (int i = 0; i < categorizedTransactions.Length; i++)
        {
            var transaction = categorizedTransactions[i];
            var transactionDate = baseDate.AddDays(random.Next(0, 30));
            
            transactions.Add(new BankTransaction(
                transactionDate,
                transaction.Title,
                transaction.Amount,
                "1234567890",
                transaction.Counterparty)
            {
                Category = transaction.Category,
                Currency = "PLN"
            });
        }

        // Add uncategorized transactions
        for (int i = 0; i < uncategorizedTransactions.Length; i++)
        {
            var transaction = uncategorizedTransactions[i];
            var transactionDate = baseDate.AddDays(random.Next(0, 30));
            
            transactions.Add(new BankTransaction(
                transactionDate,
                transaction.Title,
                transaction.Amount,
                "1234567890",
                transaction.Counterparty)
            {
                Category = transaction.Category,
                Currency = "PLN"
            });
        }

        return transactions.OrderBy(t => t.TransactionDate).ToList();
    }
}