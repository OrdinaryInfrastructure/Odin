using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.Templating.Razor;
using RazorLight;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions 
{
    /// <summary>
    /// Adds an IRazorTemplateRenderer into dependency injection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="embeddedResourcesAssembly">The assembly containing the .cshtml Razor template files as embedded resources.</param>
    /// <param name="rootNamespace">The (period separated) root namespace where the template cshtml files are embedded. With or without a trailing period.</param>
    /// <returns></returns>
    public static IServiceCollection AddRazorTemplating(this IServiceCollection services, Assembly embeddedResourcesAssembly, string rootNamespace)
    {
        services.AddLoggerAdapter();
        // Take advantage of the compiled templates cache in RazorLight. Add it as a singleton.
        services.TryAddSingleton<IRazorLightEngine>(_ => RazorLightTemplateRenderer.BuildRazorLightEngine(embeddedResourcesAssembly, rootNamespace));
        services.TryAddTransient<IRazorTemplateRenderer, RazorLightTemplateRenderer>();
        return services;
    }


}