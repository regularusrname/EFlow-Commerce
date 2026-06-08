using Catalog.API.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Infrastructure.Persistence;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("eflow_catalog-dev").HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
        builder.Property(p => p.Description).IsRequired(false);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.Price).IsRequired();
    }
}
