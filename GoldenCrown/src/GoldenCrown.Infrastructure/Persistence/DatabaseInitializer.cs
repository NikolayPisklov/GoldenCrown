using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenCrown.Infrastructure.Persistence
{
    public static class DatabaseInitializer
    {
        public static async Task MigrateDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GoldenCrownDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}
