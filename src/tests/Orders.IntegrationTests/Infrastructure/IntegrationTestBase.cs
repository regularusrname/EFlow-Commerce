namespace Orders.IntegrationTests.Infrastructure;

[Collection(nameof(IntegrationTestCollection))]
public abstract class IntegrationTestBase(OrderApiFactory factory) : IAsyncLifetime
{
    protected HttpClient Client = default!;
    protected OrderApiFactory Factory = factory;

    public async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
        Client = Factory.CreateClient();
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
