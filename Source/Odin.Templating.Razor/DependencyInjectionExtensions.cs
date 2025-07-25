using Microsoft.Extensions.DependencyInjection.Extensions;
using Odin.Templating.Razor;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions 
{
    public static IServiceCollection AddRazorTemplating(this IServiceCollection services)
    {
        services.TryAddTransient<IRazorTemplateRenderer, RazorLightTemplateRenderer>();
        return services;
    }
}