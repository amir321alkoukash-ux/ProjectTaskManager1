using Microsoft.AspNetCore.Identity;
using ProjectTaskManager.Data.Context;
using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(ApplicationDbContext context, UserManager<User> userManager)
        {
            // Seed companies if none
            if (!context.Companies.Any())
            {
                var company = new Company
                {
                    Name = "Default Company",
                    Email = "company@example.com",
                    Location = "New York"
                };
                context.Companies.Add(company);
                await context.SaveChangesAsync();

                // Seed admin user
                var adminEmail = "admin@example.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Admin",
                        DateOfBirth = new DateTime(1980, 1, 1),
                        Mobile = "1234567890",
                        CompanyId = company.Id
                    };
                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }
        }
    }
}