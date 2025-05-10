using System.Threading.Tasks;

namespace WSBInvestmentPredictor.Frontend.Shared.Services;

public interface IApiService
{
    Task<T?> PostAsync<T>(string url, object payload, string? errorContext = null);

    Task<T?> GetAsync<T>(string url, string? errorContext = null);
}