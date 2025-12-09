using Emm.Infrastructure.Data;
using Emm.Infrastructure.Data.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Emm.Infrastructure.Extensions;

public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Seed initial data for the database.
    /// Call this method in Program.cs after app.Build() and before app.Run()
    /// Example: await app.SeedDatabaseAsync();
    /// </summary>
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<XDbContext>();

        // Ensure database is created and migrated
        await context.Database.MigrateAsync();

        // Seed data in specific order (due to dependencies)
        // await UnitOfMeasureSeeder.SeedUnitOfMeasureDataAsync(context);
        // await AuthorizationSeeder.SeedAuthorizationDataAsync(context);

        // Add more seeders here as needed
        // await OtherSeeder.SeedOtherDataAsync(context);
    }
}
