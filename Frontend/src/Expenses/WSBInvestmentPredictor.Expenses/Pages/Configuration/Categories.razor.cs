using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Technology.Cqrs;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WSBInvestmentPredictor.Expenses.Pages.Configuration;

public partial class Categories : ComponentBase
{
    [Inject] private ICqrsRequestService RequestService { get; set; } = default!;
    [Inject] private Radzen.NotificationService NotificationService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IStringLocalizer<SharedResource> Loc { get; set; } = default!;

    protected List<CategoryDto> categories = new();
    protected CategoryDto? editCategory = null;
    protected string? editValue = null;
    protected string? editDescription = null;
    protected bool isLoading;
    protected string? error;
    protected string? keywordFromQuery;
    protected bool showAddDialog = false;
    protected bool showHelpDialog = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadCategories();
        await HandleQueryParameters();
    }

    protected async Task HandleQueryParameters()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = HttpUtility.ParseQueryString(uri.Query);
        keywordFromQuery = query["keyword"];
    }

    protected async Task LoadCategories()
    {
        try
        {
            isLoading = true;
            error = null;
            categories = await RequestService.Handle<GetCategories, List<CategoryDto>>(new GetCategories());
        }
        catch (Exception ex)
        {
            error = $"{Loc["ErrorLoadingData"]}: {ex.Message}";
            NotificationService.Notify(NotificationSeverity.Error, Loc["Error"], error);
            categories = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    protected void ShowAddDialog()
    {
        showAddDialog = true;
        addCategoryModel = new();
    }

    protected void HideAddDialog()
    {
        showAddDialog = false;
        addCategoryModel = new();
    }

    protected void ShowHelpDialog()
    {
        showHelpDialog = true;
    }

    protected void HideHelpDialog()
    {
        showHelpDialog = false;
    }

    protected async Task AddCategory()
    {
        if (!string.IsNullOrWhiteSpace(addCategoryModel.Name))
        {
            try
            {
                await RequestService.Handle<AddCategory, CategoryDto>(new AddCategory(addCategoryModel.Name, addCategoryModel.Description));
                addCategoryModel = new();
                HideAddDialog();
                await LoadCategories();
                NotificationService.Notify(NotificationSeverity.Success, Loc["Success"], Loc["CategoryAddedSuccessfully"]);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, Loc["Error"], ex.Message);
            }
        }
    }

    protected void StartEdit(CategoryDto category)
    {
        editCategory = category;
        editValue = category.Name;
        editDescription = category.Description;
    }

    protected async Task SaveEdit()
    {
        if (editCategory != null && !string.IsNullOrWhiteSpace(editValue))
        {
            await RequestService.Handle<UpdateCategory, CategoryDto>(new UpdateCategory(editCategory.Id, editValue, editDescription));
            editCategory = null;
            editValue = null;
            editDescription = null;
            await LoadCategories();
        }
    }

    protected void CancelEdit()
    {
        editCategory = null;
        editValue = null;
        editDescription = null;
    }

    protected async Task DeleteCategory(int categoryId)
    {
        await RequestService.Handle<DeleteCategory>(new DeleteCategory(categoryId));
        await LoadCategories();
    }

    protected void GoToRules(int categoryId)
    {
        NavigationManager.NavigateTo($"/expenses/configuration/category-rules/{categoryId}");
    }

    public class AddCategoryModel
    {
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldIsRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "CategoryNameTooLong")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "CategoryDescriptionTooLong")]
        public string? Description { get; set; }
    }
} 