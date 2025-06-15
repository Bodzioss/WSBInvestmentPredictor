using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using WSBInvestmentPredictor.Expenses.Services;
using WSBInvestmentPredictor.Expenses.Shared.Models;

namespace WSBInvestmentPredictor.Expenses.Pages;

public partial class ImportTransactions
{
    [Inject]
    public IBankTransactionService BankTransactionService { get; set; }

    [Inject]
    public ITransactionStore TransactionStore { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public NotificationService NotificationService { get; set; }

    private List<BankTransaction>? transactions;
    private bool isLoading;
    private bool isSaving;
    private string? errorMessage;

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

            using var stream = file.OpenReadStream();
            transactions = await BankTransactionService.ProcessCsvFile(stream);

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

    private void ViewTransactions()
    {
        NavigationManager.NavigateTo("/transactions");
    }
}