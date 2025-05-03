using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Reflection;
using WSBInvestmentPredictor.Frontend.Shared.Layout;

namespace WSBInvestmentPredictor.Frontend.Wasm;

public static class Defaults
{
    /// <summary>
    /// Renders link and meta tags, e.g. for stylesheets and favicon.
    /// </summary>
    public static RenderFragment Links => builder =>
    {
        builder.OpenElement(0, "link");
        builder.AddAttribute(1, "rel", "stylesheet");
        builder.AddAttribute(2, "href", "_content/Radzen.Blazor/css/material-base.css");
        builder.CloseElement();

        builder.OpenElement(3, "link");
        builder.AddAttribute(4, "rel", "icon");
        builder.AddAttribute(5, "href", "favicon.ico");
        builder.CloseElement();
    };

    /// <summary>
    /// Placeholder shown before Blazor WASM loads.
    /// </summary>
    public static RenderFragment WasmLoader => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "wasm-loader");
        builder.AddContent(2, "Loading application...");
        builder.CloseElement();
    };

    /// <summary>
    /// Injects Blazor WASM JS runtime.
    /// </summary>
    public static RenderFragment Scripts => builder =>
    {
        builder.OpenElement(0, "script");
        builder.AddAttribute(1, "src", "_framework/blazor.webassembly.js");
        builder.CloseElement();
    };

    /// <summary>
    /// Returns a full <Router> component with dynamic assemblies.
    /// </summary>
    public static RenderFragment GetDefaultRouter(Assembly mainAssembly, List<Assembly> additionalAssemblies) => builder =>
    {
        builder.OpenComponent<Router>(0);
        builder.AddAttribute(1, "AppAssembly", mainAssembly);
        builder.AddAttribute(2, "AdditionalAssemblies", additionalAssemblies.ToArray());
        builder.AddAttribute(3, "Found", (RenderFragment<RouteData>)((routeData) => builder2 =>
        {
            builder2.OpenComponent<RouteView>(0);
            builder2.AddAttribute(1, "RouteData", routeData);
            builder2.AddAttribute(2, "DefaultLayout", typeof(MainLayout));
            builder2.CloseComponent();
        }));
        builder.AddAttribute(4, "NotFound", (RenderFragment)(builder2 =>
        {
            builder2.OpenComponent<LayoutView>(0);
            builder2.AddAttribute(1, "Layout", typeof(MainLayout));
            builder2.AddAttribute(2, "ChildContent", (RenderFragment)(builder3 =>
            {
                builder3.AddContent(0, "Sorry, the page could not be found.");
            }));
            builder2.CloseComponent();
        }));
        builder.CloseComponent();
    };
}
