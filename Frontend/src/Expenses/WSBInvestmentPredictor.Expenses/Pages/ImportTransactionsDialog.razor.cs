using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using WSBInvestmentPredictor.Expenses.Services;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Frontend.Shared;
using Microsoft.Extensions.Localization;

namespace WSBInvestmentPredictor.Expenses.Pages;

/// <summary>
/// Dialog component for importing bank transactions from CSV files.
/// Provides functionality to upload, process, and save transaction data.
/// </summary>
public partial class ImportTransactionsDialog
{
    /// <summary>
    /// Service for processing bank transaction data from CSV files.
    /// </summary>
    [Inject]
    public IBankTransactionService BankTransactionService { get; set; } = default!;

    /// <summary>
    /// Service for storing and managing transaction data.
    /// </summary>
    [Inject]
    public ITransactionStore TransactionStore { get; set; } = default!;

    /// <summary>
    /// Service for displaying dialogs.
    /// </summary>
    [Inject]
    public Radzen.DialogService DialogService { get; set; } = default!;

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
    /// Service for accessing localized strings.
    /// </summary>
    [Inject]
    public IStringLocalizer<SharedResource> Loc { get; set; } = default!;

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
    /// Closes the dialog on success.
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
            
            // Close the dialog and return true to indicate success
            DialogService.Close(true);
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
    /// Closes the dialog without saving.
    /// </summary>
    private void CloseDialog()
    {
        DialogService.Close(false);
    }
} 