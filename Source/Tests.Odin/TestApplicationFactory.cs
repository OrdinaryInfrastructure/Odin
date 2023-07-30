using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Odin;

public class TestApplicationFactory : WebApplicationFactory<TestProgram>
{
    public IConfiguration GetConfiguration()
    {
        return Services.GetService<IConfiguration>();
    }
}