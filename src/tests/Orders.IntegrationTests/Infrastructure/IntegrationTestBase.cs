using MassTransit.Testing;

namespace Orders.IntegrationTests.Infrastructure;

[Collection(nameof(IntegrationTestCollection))]
public abstract class IntegrationTestBase(OrderApiFactory factory) : IAsyncLifetime
{
    protected readonly HttpClient Client = factory.CreateClient();
    protected readonly OrderApiFactory Factory = factory;
    protected readonly ITestHarness Harness = factory.Services.GetTestHarness();

    public async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    protected void Debug<T>(T value)
    {
        Console.WriteLine($"[[DEBUG]] {value}");
    }
}
