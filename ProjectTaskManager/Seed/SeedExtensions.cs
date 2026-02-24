using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProjectTaskManager.Data.Context;
using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Seed
{
    public static class SeedExtensions
    {
        public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await RoleSeeder.SeedRolesAsync(roleManager);
            await DataSeeder.SeedDataAsync(context, userManager);
        }
    }
}