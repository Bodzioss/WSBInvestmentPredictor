using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace WSBInvestmentPredictor.Frontend.Shared;

/// <summary>
/// Extension methods for configuring shared frontend services.
/// Provides centralized configuration for services used across the frontend application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds shared frontend services to the dependency injection container.
    /// Configures localization and other common services used across the application.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFrontendSharedServices(this IServiceCollection services)
    {
        // Configure localization with resources path
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        // Register string localizer for shared resources
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

/// <summary>
/// Generic string localizer implementation that wraps an IStringLocalizer.
/// Provides type-safe access to localized strings for a specific resource type.
/// </summary>
/// <typeparam name="T">The type of resource to localize</typeparam>
public class GenericStringLocalizer<T> : IStringLocalizer<T>
{
    private readonly IStringLocalizer _localizer;

    /// <summary>
    /// Initializes a new instance of the GenericStringLocalizer class.
    /// </summary>
    /// <param name="localizer">The underlying string localizer to use</param>
    public GenericStringLocalizer(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    /// <summary>
    /// Gets the localized string for the specified name.
    /// </summary>
    public LocalizedString this[string name] => _localizer[name];

    /// <summary>
    /// Gets the localized string for the specified name with formatting arguments.
    /// </summary>
    public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

    /// <summary>
    /// Gets all localized strings, optionally including parent cultures.
    /// </summary>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);
}