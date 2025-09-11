using System.Reflection;
using Odin.System;
using RazorLight;

namespace Odin.Templating.Razor;

/// <summary>
/// RazorLight implementation of IRazorTemplateRenderer
/// </summary>
public class RazorLightTemplateRenderer : IRazorTemplateRenderer
{
    private readonly IRazorLightEngine _razorLightEngine;

    /// <summary>
    /// Constructor used in DI.
    /// </summary>
    /// <param name="razorLightEngine"></param>
    public RazorLightTemplateRenderer(IRazorLightEngine razorLightEngine)
    {
        _razorLightEngine = razorLightEngine;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="embeddedResourcesAssembly">Assembly containing</param>
    /// <param name="rootNamespace">The (period separated) case-sensitive root namespace where the template cshtml files are embedded. With or without a trailing period.</param>
    public RazorLightTemplateRenderer(Assembly embeddedResourcesAssembly, string rootNamespace)
    {
        _razorLightEngine = BuildRazorLightEngine(embeddedResourcesAssembly, rootNamespace);
    }

    internal static IRazorLightEngine BuildRazorLightEngine(Assembly embeddedResourcesAssembly, string? rootNamespace = null)
    {
        return new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(embeddedResourcesAssembly, rootNamespace)
            .UseMemoryCachingProvider()
            .Build();
    }

    /// <inheritdoc />
    public async Task<ResultValue<string>> RenderAsync<TModel>(string templateKey, TModel viewModel)
    {
        try
        {
            string result = await _razorLightEngine.CompileRenderAsync<TModel>(templateKey, viewModel);
            return Result.Succeed<string>(result);
        }
        catch (Exception err)
        {
            return Result.Fail<string>(err.Message);
        }
    }
}