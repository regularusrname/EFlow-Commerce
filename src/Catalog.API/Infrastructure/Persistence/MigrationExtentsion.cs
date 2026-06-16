using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Infrastructure.Persistence;

public static class MigrationExtension
{
    public static async Task ApplyMigrationAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CatalogDbContext>>();

        var maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(5);

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                logger.LogInformation(
                    "Applying Catalog database migrations. Attemp {attempt}/{maxAttemp}",
                    attempt,
                    maxAttempts
                );
                await context.Database.MigrateAsync();

                logger.LogInformation("Catalog database migrations applied successfully");

                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(
                    ex,
                    "Failed to apply migrations. Retrying in {DelaySeconds} seconds. Attempt {Attempt}/{MaxAttempts}",
                    delay.TotalSeconds,
                    attempt,
                    maxAttempts
                );

                await Task.Delay(delay);
            }

            await context.Database.MigrateAsync();
        }
    }
}
