using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Queries;
using WSBInvestmentPredictor.Expenses.Shared.Dto;
using WSBInvestmentPredictor.Frontend.Shared;
using WSBInvestmentPredictor.Technology.Cqrs;
using System.ComponentModel.DataAnnotations;

namespace WSBInvestmentPredictor.Expenses.Pages.Configuration;

public partial class Categories : ComponentBase
{
    [Inject] private ICqrsRequestService RequestService { get; set; } = default!;
    [Inject] private Radzen.NotificationService NotificationService { get; set; } = default!;
    [Inject] private IStringLocalizer<SharedResource> Loc { get; set; } = default!;

    protected List<CategoryDto> categories = new();
    protected CategoryDto? editCategory = null;
    protected string? editValue = null;
    protected string? editDescription = null;
    protected bool isLoading;
    protected string? error;

    protected override async Task OnInitializedAsync()
    {
        await LoadCategories();
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

    protected async Task AddCategory()
    {
        if (!string.IsNullOrWhiteSpace(addCategoryModel.Name))
        {
            await RequestService.Handle<AddCategory, CategoryDto>(new AddCategory(addCategoryModel.Name, addCategoryModel.Description));
            addCategoryModel.Name = string.Empty;
            addCategoryModel.Description = null;
            await LoadCategories();
            StateHasChanged();
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

    public class AddCategoryModel
    {
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "FieldIsRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "CategoryNameTooLong")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "CategoryDescriptionTooLong")]
        public string? Description { get; set; }
    }
} 