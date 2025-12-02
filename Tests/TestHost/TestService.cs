namespace TestHost;

public interface ITestService
{
    Task<string> HelloWorld();
}

public class TestService : ITestService
{
    public async Task<string> HelloWorld()
    {
        return await Task.FromResult("Hello World");
    }
}