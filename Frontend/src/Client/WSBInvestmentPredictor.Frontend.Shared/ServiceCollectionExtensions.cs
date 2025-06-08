using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace WSBInvestmentPredictor.Frontend.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrontendSharedServices(this IServiceCollection services)
    {
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        services.AddSingleton<IStringLocalizer<SharedResource>>(sp =>
        {
            var factory = sp.GetRequiredService<IStringLocalizerFactory>();
            var assembly = typeof(SharedResource).Assembly;
            var baseName = "WSBInvestmentPredictor.Frontend.Shared.SharedResource";
            var assemblyName = assembly.GetName().Name;

            var localizer = factory.Create(baseName, assemblyName);
            return new GenericStringLocalizer<SharedResource>(localizer);
        });

        return services;
    }
}

public class GenericStringLocalizer<T> : IStringLocalizer<T>
{
    private readonly IStringLocalizer _localizer;

    public GenericStringLocalizer(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    public LocalizedString this[string name] => _localizer[name];

    public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);
}