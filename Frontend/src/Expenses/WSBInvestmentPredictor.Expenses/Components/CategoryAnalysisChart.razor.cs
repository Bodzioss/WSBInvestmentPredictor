using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WSBInvestmentPredictor.Expenses.Shared.Dto;

namespace WSBInvestmentPredictor.Expenses.Components;

public partial class CategoryAnalysisChart : ComponentBase, IDisposable
{
    [Parameter]
    public List<CategoryAnalysisDto>? Analysis { get; set; }

    [Parameter]
    public string? SelectedCategory { get; set; }

    [Parameter]
    public EventCallback<string?> OnCategorySelected { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private DotNetObjectReference<CategoryAnalysisChart>? dotNetHelper;

    private int TotalTransactions => Analysis?.Sum(c => c.TransactionCount) ?? 0;
    private int UncategorizedTransactions => Analysis?.FirstOrDefault(c => c.CategoryName == "Uncategorized")?.TransactionCount ?? 0;
    private int CategorizedTransactions => Analysis?.Where(c => c.CategoryName != "Uncategorized").Sum(c => c.TransactionCount) ?? 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetHelper = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("charts.setupFilterHandler", dotNetHelper);
        }

        if (Analysis != null && Analysis.Any())
        {
            await RenderChart();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Analysis != null && Analysis.Any())
        {
            await RenderChart();
        }
    }

    private async Task RenderChart()
    {
        if (Analysis == null) return;

        try
        {
            // Destroy existing chart first
            await JSRuntime.InvokeVoidAsync("window.destroyChart", "categoryChart");

            var chartData = new
            {
                labels = Analysis.Select(c => c.CategoryName).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        data = Analysis.Select(c => c.TransactionCount).ToArray(),
                        backgroundColor = GenerateColors(Analysis.Count),
                        borderWidth = 2,
                        borderColor = "#fff"
                    }
                }
            };

            await JSRuntime.InvokeVoidAsync("charts.renderPieChart", "categoryChart", chartData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering chart: {ex.Message}");
        }
    }

    private string[] GenerateColors(int count)
    {
        var colors = new[]
        {
            "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF",
            "#FF9F40", "#FF6384", "#C9CBCF", "#4BC0C0", "#FF6384"
        };

        var result = new string[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = colors[i % colors.Length];
        }
        return result;
    }

    private async Task ClearFilter()
    {
        await OnCategorySelected.InvokeAsync(null);
    }

    [JSInvokable]
    public async Task OnPieChartCategorySelected(string? category)
    {
        await OnCategorySelected.InvokeAsync(category);
    }

    public void Dispose()
    {
        try
        {
            // Destroy chart before disposing
            _ = JSRuntime.InvokeVoidAsync("window.destroyChart", "categoryChart");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error destroying chart in Dispose: {ex.Message}");
        }
        
        dotNetHelper?.Dispose();
    }
} 