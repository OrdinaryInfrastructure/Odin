using Odin.System;

namespace Odin.Templating.Razor;

/// <summary>
/// Provides OrdINary use case rendering of embedded '.cshtml' templates using Razor syntax.
/// </summary>
public interface IRazorTemplateRenderer
{
    /// <summary>
    /// Compiles and caches the template, and renders the viewModel into it, return the result.
    /// Does not throw exceptions, but return a failed outcome if unsuccessful.
    /// </summary>
    /// <param name="templateKey">Case-sensitive unique name of the template. For embedded cshtml files this is the name of the file with the '.cshtml'</param>
    /// <param name="viewModel"></param>
    /// <typeparam name="TModel">The type of the viewModel</typeparam>
    /// <returns>Success or failure. Is success, the Outcome's Value contains the rendered template.</returns>
    Task<Outcome<string>> RenderAsync<TModel>(string templateKey, TModel viewModel);
}