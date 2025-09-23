using Microsoft.EntityFrameworkCore;

namespace ASM_01.DataAccessLayer;

public static class MigrateDatabase
{
    /// <summary>
    /// Ensures database is created and applies all pending migrations.
    /// Call this once on server startup.
    /// </summary>
    /// <typeparam name="TContext">Your DbContext type</typeparam>
    /// <param name="context">The DbContext instance</param>
    public static async Task ApplyMigrations<TContext>(TContext context) where TContext : DbContext
    {
        // This will create the database if it doesn't exist
        // and apply any pending migrations automatically
        await context.Database.MigrateAsync();
    }
}
