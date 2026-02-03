using Microsoft.Extensions.DependencyInjection;

namespace ProjectTaskManager.Data.Seed
{
    public static class SeedExtensions
    {
        public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            await DataSeeder.SeedData(scope.ServiceProvider);
        }
    }
}