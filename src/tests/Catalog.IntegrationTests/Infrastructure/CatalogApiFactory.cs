using Catalog.API.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Catalog.IntegrationTests.Infrastructure;

public class CatalogApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgre = new PostgreSqlBuilder("postgres:16")
        .WithDatabase("eflow-test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    private CatalogDbContext _context = null!;
    private Respawner _respawner = null!;

    public async Task InitializeAsync()
    {
        await _postgre.StartAsync();

        var dbOptions = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseNpgsql(_postgre.GetConnectionString())
            .Options;

        _context = new CatalogDbContext(dbOptions);

        await _context.Database.MigrateAsync();

        using var conn = new NpgsqlConnection(_postgre.GetConnectionString());
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            conn,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
                TablesToIgnore = ["__EFMigrationHistory"],
            }
        );
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<CatalogDbContext>>();
            services.RemoveAll<CatalogDbContext>();
            services.AddDbContext<CatalogDbContext>(opts =>
            {
                opts.UseNpgsql(_postgre.GetConnectionString());
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var conn = new NpgsqlConnection(_postgre.GetConnectionString());
        await conn.OpenAsync();

        await _respawner.ResetAsync(conn);
        _context.ChangeTracker.Clear();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgre.StopAsync();
        await _postgre.DisposeAsync();
        await _context.DisposeAsync();
    }
}
