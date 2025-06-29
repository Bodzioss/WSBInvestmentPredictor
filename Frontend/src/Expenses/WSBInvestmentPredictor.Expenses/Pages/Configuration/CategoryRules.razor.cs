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
    [Inject] private ICqrsRequestService RequestService { get; set; } = default!;
    [Inject] private Radzen.NotificationService NotificationService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IStringLocalizer<SharedResource> Loc { get; set; } = default!;

    protected List<CategoryRuleDto> rules = new();
    protected List<CategoryDto> categories = new();
    protected CategoryRuleFormModel formModel = new();
    protected CategoryRuleDto? editRule = null;
    protected bool isLoading;
    protected string? error;

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

    protected async Task LoadData()
    {
        try
        {
            isLoading = true;
            error = null;
            rules = await RequestService.Handle<GetCategoryRules, List<CategoryRuleDto>>(new GetCategoryRules());
            categories = await RequestService.Handle<GetCategories, List<CategoryDto>>(new GetCategories());
        }
        catch (Exception ex)
        {
            error = $"{Loc["ErrorLoadingData"]}: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, Loc["Error"], error);
            rules = new();
            categories = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            if (editRule == null)
            {
                await RequestService.Handle<AddCategoryRule, CategoryRuleDto>(new AddCategoryRule(formModel.Keyword, formModel.CategoryId, formModel.FieldType));
            }
            else
            {
                await RequestService.Handle<UpdateCategoryRule, CategoryRuleDto>(new UpdateCategoryRule(formModel.Id, formModel.Keyword, formModel.CategoryId, formModel.FieldType));
            }
            formModel = new();
            editRule = null;
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
        formModel = new CategoryRuleFormModel 
        { 
            Id = rule.Id, 
            Keyword = rule.Keyword, 
            CategoryId = rule.CategoryId,
            FieldType = rule.FieldType
        };
        editRule = rule;
    }

    protected async Task DeleteRule(int id)
    {
        try
        {
            await RequestService.Handle<DeleteCategoryRule>(new DeleteCategoryRule(id));
            await LoadData();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            error = $"{Loc["ErrorDeletingRule"]}: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, Loc["Error"], error);
        }
    }

    protected void CancelEdit()
    {
        formModel = new();
        editRule = null;
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