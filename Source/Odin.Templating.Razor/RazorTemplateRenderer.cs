using Odin.System;

namespace Odin.Templating.Razor;

public class RazorLightTemplateRenderer : IRazorTemplateRenderer
{
    public async Task<Outcome<string>> RenderAsync<TModel>(string templateKey, TModel viewModel)
    {
        return Outcome.Succeed<string>($"Todo: Render viewModel with template: {templateKey}");
    }
}