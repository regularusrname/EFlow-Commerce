using System.Reflection;
using Catalog.API.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Infrastructure.Persistence;

public class CatalogDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}
