using System.Reflection;
using NUnit.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Odin.Templating.Razor;
using RazorLight;

namespace Tests.Odin.Templating.Razor
{
    [TestFixture]
    public sealed class DependencyInjectionExtensionTests
    {
        [Test]
        public void AddRazorTemplating_succeeds()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            Assembly testsAssembly = typeof(RazorTemplateRendererTests).Assembly;
            builder.Services.AddOdinRazorTemplating(testsAssembly, "Tests.Odin.Templating.Razor");
            WebApplication sut = builder.Build();
            
            IRazorTemplateRenderer? result = sut.Services.GetService<IRazorTemplateRenderer>();
            IRazorLightEngine? dependency = sut.Services.GetService<IRazorLightEngine>();
            
            Assert.That(result, Is.Not.Null);
            Assert.That(dependency, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RazorLightTemplateRenderer>());
        }
        
    }
}