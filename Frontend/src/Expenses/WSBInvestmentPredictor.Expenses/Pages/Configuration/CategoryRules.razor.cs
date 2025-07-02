using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using System.Web;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Technology.Cqrs;
using WSBInvestmentPredictor.Expenses.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace WSBInvestmentPredictor.Expenses.Pages.Configuration;

public partial class CategoryRules : ComponentBase
{
    [Parameter] public int? CategoryId { get; set; }
    
    [Inject] private ICqrsRequestService RequestService { get; set; } = default!;
    [Inject] private Radzen.NotificationService NotificationService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IStringLocalizer<SharedResource> Loc { get; set; } = default!;

    protected List<CategoryRuleDto> rules = new();
    protected CategoryDto? selectedCategory = null;
    protected CategoryRuleFormModel formModel = new();
    protected CategoryRuleDto? editRule = null;
    protected bool isLoading;
    protected string? error;
    protected bool showDialog = false;
    protected bool showHelpDialog = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
        // Handle query string to pre-fill keyword
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var keyword = query["keyword"];
        if (!string.IsNullOrEmpty(keyword))
        {
            formModel.Keyword = keyword;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (CategoryId.HasValue)
        {
            await LoadData();
        }
    }

    protected async Task LoadData()
    {
        try
        {
            isLoading = true;
            error = null;
            
            if (CategoryId.HasValue)
            {
                // Load rules for specific category
                var allRules = await RequestService.Handle<GetCategoryRules, List<CategoryRuleDto>>(new GetCategoryRules());
                rules = allRules.Where(r => r.CategoryId == CategoryId.Value).ToList();
                
                // Load category details
                var categories = await RequestService.Handle<GetCategories, List<CategoryDto>>(new GetCategories());
                selectedCategory = categories.FirstOrDefault(c => c.Id == CategoryId.Value);
                
                // Pre-fill category ID in form
                formModel.CategoryId = CategoryId.Value;
            }
            else
            {
                // Load all rules (for backward compatibility)
                rules = await RequestService.Handle<GetCategoryRules, List<CategoryRuleDto>>(new GetCategoryRules());
            }
        }
        catch (Exception ex)
        {
            error = $"{Loc["ErrorLoadingData"]}: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, Loc["Error"], error);
            rules = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    protected void ShowAddDialog()
    {
        showDialog = true;
        editRule = null;
        formModel = new() { CategoryId = CategoryId ?? 0 };
    }

    protected void HideDialog()
    {
        showDialog = false;
        editRule = null;
        formModel = new() { CategoryId = CategoryId ?? 0 };
    }

    protected void ShowHelpDialog()
    {
        showHelpDialog = true;
    }

    protected void HideHelpDialog()
    {
        showHelpDialog = false;
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            if (editRule == null)
            {
                await RequestService.Handle<AddCategoryRule, CategoryRuleDto>(new AddCategoryRule(formModel.Keyword, formModel.CategoryId, formModel.FieldType));
                NotificationService.Notify(NotificationSeverity.Success, Loc["Success"], Loc["RuleAddedAndAppliedSuccessfully"]);
            }
            else
            {
                await RequestService.Handle<UpdateCategoryRule, CategoryRuleDto>(new UpdateCategoryRule(formModel.Id, formModel.Keyword, formModel.CategoryId, formModel.FieldType));
                NotificationService.Notify(NotificationSeverity.Success, Loc["Success"], Loc["RuleUpdatedAndAppliedSuccessfully"]);
            }
            HideDialog();
            await LoadData();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            error = $"{Loc["ErrorSavingRule"]}: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, Loc["Error"], error);
        }
    }

    protected void EditRule(CategoryRuleDto rule)
    {
        editRule = rule;
        formModel = new CategoryRuleFormModel 
        { 
            Id = rule.Id, 
            Keyword = rule.Keyword, 
            CategoryId = rule.CategoryId,
            FieldType = rule.FieldType
        };
        showDialog = true;
    }

    protected async Task DeleteRule(int id)
    {
        try
        {
            await RequestService.Handle<DeleteCategoryRule>(new DeleteCategoryRule(id));
            await LoadData();
            NotificationService.Notify(NotificationSeverity.Success, Loc["Success"], Loc["RuleDeletedAndAppliedSuccessfully"]);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            error = $"{Loc["ErrorDeletingRule"]}: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, Loc["Error"], error);
        }
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo("/expenses/configuration/categories");
    }

    public class CategoryRuleFormModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldIsRequired")]
        [StringLength(200, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "KeywordTooLong")]
        public string Keyword { get; set; } = string.Empty;
        
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "CategoryIsRequired")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "YouMustSelectCategory")]
        public int CategoryId { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldIsRequired")]
        public TransactionFieldType FieldType { get; set; } = TransactionFieldType.Title;
    }
} 