using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Data.Context;
using ProjectTaskManager.Entities;


namespace ProjectTaskManager.Data.Seed
{
    public static class DataSeeder
    {
        public static async Entities.TaskRecord SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await SeedCompanies(context);
            await SeedEmployees(context);
            await SeedProjects(context);
            await SeedTasks(context);
            await SeedProjectEmployees(context);

            await context.SaveChangesAsync();
        }

        private static async Entities.TaskRecord SeedCompanies(ApplicationDbContext context)
        {
            if (!await context.Companies.AnyAsync())
            {
                var companies = new List<Company>
                {
                    new Company
                    {
                        Name = "TechCorp Solutions",
                        Email = "info@techcorp.com",
                        Location = "New York, USA",
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    },
                    new Company
                    {
                        Name = "InnovateSoft",
                        Email = "contact@innovatesoft.com",
                        Location = "London, UK",
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    },
                    new Company
                    {
                        Name = "GlobalSystems",
                        Email = "support@globalsystems.com",
                        Location = "Tokyo, Japan",
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    }
                };

                await context.Companies.AddRangeAsync(companies);
                Console.WriteLine("Seeded Companies");
            }
        }

        private static async Task SeedEmployees(ApplicationDbContext context)
        {
            if (!await context.Employees.AnyAsync())
            {
                var company1 = await context.Companies.FirstAsync(c => c.Name == "TechCorp Solutions");
                var company2 = await context.Companies.FirstAsync(c => c.Name == "InnovateSoft");
                var company3 = await context.Companies.FirstAsync(c => c.Name == "GlobalSystems");

                var employees = new List<Employee>
                {
                    // TechCorp Employees
                    new Employee
                    {
                        FirstName = "John",
                        LastName = "Smith",
                        Email = "john.smith@techcorp.com",
                        DateOfBirth = new DateTime(1985, 5, 15),
                        Mobile = "+1-555-0101",
                        CompanyId = company1.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-29)
                    },
                    new Employee
                    {
                        FirstName = "Sarah",
                        LastName = "Johnson",
                        Email = "sarah.j@techcorp.com",
                        DateOfBirth = new DateTime(1990, 8, 22),
                        Mobile = "+1-555-0102",
                        CompanyId = company1.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-28)
                    },
                    
                    // InnovateSoft Employees
                    new Employee
                    {
                        FirstName = "David",
                        LastName = "Brown",
                        Email = "david.b@innovatesoft.com",
                        DateOfBirth = new DateTime(1988, 3, 10),
                        Mobile = "+44-555-0101",
                        CompanyId = company2.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-24)
                    },
                    
                    // GlobalSystems Employees
                    new Employee
                    {
                        FirstName = "Yuki",
                        LastName = "Tanaka",
                        Email = "yuki.t@globalsystems.com",
                        DateOfBirth = new DateTime(1992, 11, 5),
                        Mobile = "+81-555-0101",
                        CompanyId = company3.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-19)
                    }
                };

                await context.Employees.AddRangeAsync(employees);
                Console.WriteLine("Seeded Employees");
            }
        }

        private static async Task SeedProjects(ApplicationDbContext context)
        {
            if (!await context.Projects.AnyAsync())
            {
                var company1 = await context.Companies.FirstAsync(c => c.Name == "TechCorp Solutions");
                var company2 = await context.Companies.FirstAsync(c => c.Name == "InnovateSoft");

                var projects = new List<Project>
                {
                    new Project
                    {
                        Name = "E-Commerce Platform",
                        Description = "Build a modern e-commerce platform with AI recommendations",
                        CompanyId = company1.Id,
                        StartDate = DateTime.UtcNow.AddDays(-20),
                        DueDate = DateTime.UtcNow.AddDays(30),
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new Project
                    {
                        Name = "Mobile Banking App",
                        Description = "Develop a secure mobile banking application",
                        CompanyId = company1.Id,
                        StartDate = DateTime.UtcNow.AddDays(-15),
                        DueDate = DateTime.UtcNow.AddDays(45),
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    },
                    new Project
                    {
                        Name = "CRM System",
                        Description = "Customer Relationship Management system for sales team",
                        CompanyId = company2.Id,
                        StartDate = DateTime.UtcNow.AddDays(-10),
                        DueDate = DateTime.UtcNow.AddDays(60),
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    }
                };

                await context.Projects.AddRangeAsync(projects);
                Console.WriteLine("Seeded Projects");
            }
        }

        private static async Task SeedTasks(ApplicationDbContext context)
        {
            if (!await context.Tasks.AnyAsync())
            {
                var project1 = await context.Projects.FirstAsync(p => p.Name == "E-Commerce Platform");
                var project2 = await context.Projects.FirstAsync(p => p.Name == "Mobile Banking App");
                var employee1 = await context.Employees.FirstAsync(e => e.Email == "john.smith@techcorp.com");
                var employee2 = await context.Employees.FirstAsync(e => e.Email == "sarah.j@techcorp.com");

                var tasks = new List<Task>
                {
                    new Task
                    {
                        Name = "Design Database Schema",
                        Description = "Create ER diagram and database tables",
                        ProjectId = project1.Id,
                        EmployeeId = employee1.Id,
                        DueDate = DateTime.UtcNow.AddDays(5),
                        CreatedAt = DateTime.UtcNow.AddDays(-18)
                    },
                    new Task
                    {
                        Name = "Implement User Authentication",
                        Description = "Add login, registration, and password reset functionality",
                        ProjectId = project1.Id,
                        EmployeeId = employee2.Id,
                        DueDate = DateTime.UtcNow.AddDays(10),
                        CreatedAt = DateTime.UtcNow.AddDays(-17)
                    },
                    new Task
                    {
                        Name = "Create API Endpoints",
                        Description = "Develop RESTful APIs for product management",
                        ProjectId = project1.Id,
                        DueDate = DateTime.UtcNow.AddDays(15),
                        CreatedAt = DateTime.UtcNow.AddDays(-16)
                    },
                    new Task
                    {
                        Name = "Security Audit",
                        Description = "Perform security assessment and fix vulnerabilities",
                        ProjectId = project2.Id,
                        EmployeeId = employee1.Id,
                        DueDate = DateTime.UtcNow.AddDays(20),
                        CreatedAt = DateTime.UtcNow.AddDays(-12)
                    }
                };

                await context.Tasks.AddRangeAsync(tasks);
                Console.WriteLine("Seeded Tasks");
            }
        }

        private static async Task SeedProjectEmployees(ApplicationDbContext context)
        {
            if (!await context.ProjectEmployees.AnyAsync())
            {
                var project1 = await context.Projects.FirstAsync(p => p.Name == "E-Commerce Platform");
                var project2 = await context.Projects.FirstAsync(p => p.Name == "Mobile Banking App");
                var employee1 = await context.Employees.FirstAsync(e => e.Email == "john.smith@techcorp.com");
                var employee2 = await context.Employees.FirstAsync(e => e.Email == "sarah.j@techcorp.com");

                var projectEmployees = new List<ProjectEmployee>
                {
                    new ProjectEmployee
                    {
                        ProjectId = project1.Id,
                        EmployeeId = employee1.Id,
                        AssignedDate = DateTime.UtcNow.AddDays(-18)
                    },
                    new ProjectEmployee
                    {
                        ProjectId = project1.Id,
                        EmployeeId = employee2.Id,
                        AssignedDate = DateTime.UtcNow.AddDays(-17)
                    },
                    new ProjectEmployee
                    {
                        ProjectId = project2.Id,
                        EmployeeId = employee1.Id,
                        AssignedDate = DateTime.UtcNow.AddDays(-12)
                    }
                };

                await context.ProjectEmployees.AddRangeAsync(projectEmployees);
                Console.WriteLine("Seeded ProjectEmployees");
            }
        }
    }
}