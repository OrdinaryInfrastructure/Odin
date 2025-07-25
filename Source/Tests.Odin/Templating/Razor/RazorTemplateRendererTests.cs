using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Odin.Email;
using Odin.System;
using Odin.Templating.Razor;

namespace Tests.Odin.Templating.Razor
{
    [TestFixture]
    public sealed class RazorTemplateRendererTests : IntegrationTest
    {
        private string _toTestEmail;
        private string _fromTestEmail;

        [SetUp]
        public void Setup()
        {
            IConfiguration config = AppFactory.GetConfiguration();
            _toTestEmail = config["Email-TestToAddress"];
            _fromTestEmail = config["Email-TestFromAddress"];
        }


        [Test]
        [TestCase("Tests.Odin.Templating.Razor", "TestTemplate1", true, Description = "Without a period")]
        [TestCase("tests.odin.templating.razor", "TestTemplate1", false, Description = "Case sensitive namespace..")]
        [TestCase("Tests.Odin.Templating.Razor.", "TestTemplate1", true, Description = "With a period")]
        [TestCase("tests.odin.templating.razor", "testtemplate1", false, Description = "Case sensitive template")]
        [TestCase("Wrong", "TestTemplate1", false, Description = "Wrong namespace")]
        [TestCase("Tests.Odin.Templating.Razor", "Wrong", false, Description = "Wrong templateKey")]
        [TestCase("Tests.Odin.Templating", "TestTemplate1", false, Description = "Testing sub dirs - they don't work")]
        [TestCase(null, "TestTemplate1", false, Description = "Testing optionality of namespace. Doesn't work.")]
        [TestCase("", "TestTemplate1", false, Description = "Testing optionality of namespace. Doesn't work.")]
        public async Task Create_with_different_templateKeys_and_namespaces(string rootNamespace, string templateKey, bool shouldSucceed)
        {
            Assembly testsAssembly = typeof(RazorTemplateRendererTests).Assembly;
            RazorLightTemplateRenderer sut = new RazorLightTemplateRenderer(testsAssembly, rootNamespace);
            
            Outcome<string> result = await sut.RenderAsync(templateKey, new TestViewModel(){ Title = "World"});

            Assert.That(result.Success, Is.EqualTo(shouldSucceed));
            if (shouldSucceed)
            {
                Assert.That(result.Value, Does.Contain("<div>Hello World</div>"), result.MessagesToString());
            }
            else
            {
                Assert.That(result.MessagesToString(), Is.Not.Empty);
            }
        }

    }
}