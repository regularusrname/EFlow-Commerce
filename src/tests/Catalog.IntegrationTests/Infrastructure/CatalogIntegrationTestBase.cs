namespace Catalog.IntegrationTests.Infrastructure;

[Collection(nameof(IntegrationTestCollection))]
public abstract class CatalogIntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly CatalogApiFactory Factory;

    protected CatalogIntegrationTestBase(CatalogApiFactory factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();
    }
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }

    protected void Debug<T>(T valueForDebug)
    {
        Console.WriteLine($"[[DEBUG]]: {valueForDebug}");
    }
}
