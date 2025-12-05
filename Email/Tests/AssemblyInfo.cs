
// In your test project's AssemblyInfo.cs or a similar file
using Microsoft.AspNetCore.Mvc.Testing;
using System.Reflection;

[assembly: WebApplicationFactoryContentRoot("Tests.Odin.Email.csproj", "Tests.Odin.Email.dll", "/", "1")]
// If you have multiple content roots, you can add more attributes with different keys or priorities.
// [assembly: WebApplicationFactoryContentRoot("AnotherWebApp.csproj", "AnotherWebApp", "AnotherWebApp.csproj", Priority = 1)]



