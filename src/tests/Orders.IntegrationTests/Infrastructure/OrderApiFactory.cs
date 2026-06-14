using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Orders.API.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Respawn;
using Npgsql;
using Orders.API.Common.Abstractions;
using Orders.API.Common;
using Orders.API.Infrastructure.Catalog;

namespace Orders.IntegrationTests.Infrastructure;

public class OrderApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16")
        .WithDatabase("eflow-test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private Respawner _respawner = null!;

    private OrderDbContext _context = null!;

    public FakeCatalogClient CatalogClient { get; } = new();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;
        _context = new OrderDbContext(options);

        await _context.Database.MigrateAsync();

        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public new async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await _postgres.DisposeAsync();
        await _context.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());

        await conn.OpenAsync();

        await _respawner.ResetAsync(conn);
        _context.ChangeTracker.Clear();

        CatalogClient.Clear();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services => 
        {
            services.RemoveAll<OrderDbContext>();
            services.RemoveAll<DbContextOptions<OrderDbContext>>();
            services.RemoveAll<ICatalogClient<Result<CatalogProductResponse>>>();

            services.AddSingleton<ICatalogClient<Result<CatalogProductResponse>>>(CatalogClient);
            services.AddDbContext<OrderDbContext>(opts => 
            {
                opts.UseNpgsql(_postgres.GetConnectionString());
            });
        });
    }
}
