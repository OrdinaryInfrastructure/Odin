namespace Tests.Odin;

/// <summary>
///  Base class for integration tests or any test needing config
/// </summary>
public abstract class IntegrationTest
{
    protected readonly TestApplicationFactory AppFactory;

    protected IntegrationTest()
    {
        AppFactory = new TestApplicationFactory();
    }
}