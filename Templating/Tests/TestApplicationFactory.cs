using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Odin.Templating;

public class TestApplicationFactory : WebApplicationFactory<Program>
{
    public IConfiguration GetConfiguration()
    {
        return Services.GetRequiredService<IConfiguration>();
    }
}