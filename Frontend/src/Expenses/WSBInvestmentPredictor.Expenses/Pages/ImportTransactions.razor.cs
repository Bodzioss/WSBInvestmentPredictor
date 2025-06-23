using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using WSBInvestmentPredictor.Expenses.Services;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Technology.Cqrs;

namespace WSBInvestmentPredictor.Expenses.Pages;

/// <summary>
/// Component for importing bank transactions from CSV files.
/// Provides functionality to upload, process, and save transaction data.
/// </summary>
public partial class ImportTransactions
{
    /// <summary>
    /// Service for processing bank transaction data from CSV files.
    /// </summary>
    [Inject]
    public IBankTransactionService BankTransactionService { get; set; }

    /// <summary>
    /// Service for storing and managing transaction data.
    /// </summary>
    [Inject]
    public ITransactionStore TransactionStore { get; set; }

    /// <summary>
    /// Service for managing navigation between pages.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Service for handling CQRS requests.
    /// </summary>
    [Inject]
    public ICqrsRequestService RequestService { get; set; } = default!;

    /// <summary>
    /// Service for displaying notifications to the user.
    /// </summary>
    [Inject]
    public Radzen.NotificationService NotificationService { get; set; } = default!;

    /// <summary>
    /// Collection of processed transactions from the uploaded file.
    /// </summary>
    private List<BankTransaction>? transactions;

    /// <summary>
    /// Indicates if file processing is in progress.
    /// </summary>
    private bool isLoading;

    /// <summary>
    /// Indicates if transaction saving is in progress.
    /// </summary>
    private bool isSaving;

    /// <summary>
    /// Error message if an operation fails.
    /// </summary>
    private string? errorMessage;

    /// <summary>
    /// Handles file upload and processes the CSV file to extract transactions.
    /// </summary>
    /// <param name="e">Event arguments containing the uploaded file information</param>
    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        try
        {
            isLoading = true;
            errorMessage = null;
            transactions = null;

            var file = e.File;
            if (file == null)
            {
                errorMessage = "No file selected";
                return;
            }

            // Process the uploaded CSV file
            using var stream = file.OpenReadStream();
            transactions = await BankTransactionService.ProcessCsvFile(stream);

            // Log processing results for debugging
            Console.WriteLine($"Loaded {transactions.Count} transactions from file");
            if (transactions.Any())
            {
                var first = transactions.First();
                Console.WriteLine($"First transaction: Date={first.TransactionDate}, Amount={first.Amount}, Account={first.Account}");
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error processing file: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, "Error", errorMessage);
        }
        finally
        {
            isLoading = false;
        }
    }

    /// <summary>
    /// Saves the processed transactions to the database.
    /// Navigates to the transactions view on success.
    /// </summary>
    private async Task SaveTransactions()
    {
        if (transactions == null || !transactions.Any())
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Warning", "No transactions to save");
            return;
        }

        try
        {
            isSaving = true;
            await TransactionStore.AddTransactions(transactions);
            NotificationService.Notify(NotificationSeverity.Success, "Success", $"Successfully saved {transactions.Count} transactions");
            NavigationManager.NavigateTo("/transactions");
        }
        catch (Exception ex)
        {
            errorMessage = $"Error saving transactions: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, "Error", errorMessage);
        }
        finally
        {
            isSaving = false;
        }
    }

    /// <summary>
    /// Navigates to the transactions view page.
    /// </summary>
    private void ViewTransactions()
    {
        NavigationManager.NavigateTo("/transactions");
    }
}