using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using WSBInvestmentPredictor.Expenses.Shared.Models;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Dto;

namespace WSBInvestmentPredictor.Expenses.Pages;

public partial class CategorizeTransactions : ComponentBase
{
    [Inject] private ICqrsRequestService RequestService { get; set; } = default!;
    [Inject] private Radzen.NotificationService NotificationService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IStringLocalizer<SharedResource> Loc { get; set; } = default!;

    protected IEnumerable<BankTransaction>? transactions;
    protected List<CategoryDto> categories = new();
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
            
            // Load uncategorized transactions
            transactions = await RequestService.Handle<GetUncategorizedTransactions, List<BankTransaction>>(new GetUncategorizedTransactions());
            
            // Load categories
            categories = await RequestService.Handle<GetCategories, List<CategoryDto>>(new GetCategories());
        }
        catch (Exception ex)
        {
            error = $"{Loc["ErrorLoadingData"]}: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, Loc["Error"], error);
            transactions = Enumerable.Empty<BankTransaction>();
            categories = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    protected async Task ApplyRules()
    {
        try
        {
            isLoading = true;
            await RequestService.Handle<ApplyCategoryRules, object>(new ApplyCategoryRules());
            NotificationService.Notify(NotificationSeverity.Success, Loc["Success"], Loc["RulesAppliedSuccessfully"]);
            await LoadData();
        }
        catch (Exception ex)
        {
            error = $"{Loc["ErrorApplyingRules"]}: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, Loc["Error"], error);
        }
        finally
        {
            isLoading = false;
        }
    }

    protected async Task AssignCategory(BankTransaction tx, int categoryId)
    {
        if (categoryId > 0)
        {
            await RequestService.Handle<AssignCategoryToTransaction>(new AssignCategoryToTransaction(tx.Id, categoryId));
            transactions = transactions?.Where(t => t.Id != tx.Id).ToList();
            StateHasChanged();
        }
    }

    protected void GoToAddRule(string keyword)
    {
        NavigationManager.NavigateTo($"/expenses/configuration/category-rules?keyword={Uri.EscapeDataString(keyword)}");
    }
} 