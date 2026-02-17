using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Data;
using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Data.Seed
{
    public static class DataSeeder 
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed in order (due to foreign key constraints)
            await SeedCompanies(context);
            await SeedEmployees(context);
            await SeedProjects(context);
            await SeedTasks(context);
            await SeedProjectEmployees(context);

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Database seeding completed!");
        }

        private static async Task SeedCompanies(ApplicationDbContext context)
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
                        CreatedAt = DateTime.UtcNow.AddDays(-30),
                        UpdatedAt = DateTime.UtcNow.AddDays(-30)
                    },
                    new Company
                    {
                        Name = "InnovateSoft",
                        Email = "contact@innovatesoft.com",
                        Location = "London, UK",
                        CreatedAt = DateTime.UtcNow.AddDays(-25),
                        UpdatedAt = DateTime.UtcNow.AddDays(-25)
                    },
                    new Company
                    {
                        Name = "GlobalSystems",
                        Email = "support@globalsystems.com",
                        Location = "Tokyo, Japan",
                        CreatedAt = DateTime.UtcNow.AddDays(-20),
                        UpdatedAt = DateTime.UtcNow.AddDays(-20)
                    }
                };

                await context.Companies.AddRangeAsync(companies);
                Console.WriteLine("✅ Seeded 3 Companies");
            }
        }

        private static async Task SeedEmployees(ApplicationDbContext context)
        {
            if (!await context.Employees.AnyAsync())
            {
                var companies = await context.Companies.ToListAsync();
                var company1 = companies[0]; // TechCorp
                var company2 = companies[1]; // InnovateSoft
                var company3 = companies[2]; // GlobalSystems

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
                        CreatedAt = DateTime.UtcNow.AddDays(-29),
                        UpdatedAt = DateTime.UtcNow.AddDays(-29)
                    },
                    new Employee
                    {
                        FirstName = "Sarah",
                        LastName = "Johnson",
                        Email = "sarah.j@techcorp.com",
                        DateOfBirth = new DateTime(1990, 8, 22),
                        Mobile = "+1-555-0102",
                        CompanyId = company1.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-28),
                        UpdatedAt = DateTime.UtcNow.AddDays(-28)
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
                        CreatedAt = DateTime.UtcNow.AddDays(-24),
                        UpdatedAt = DateTime.UtcNow.AddDays(-24)
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
                        CreatedAt = DateTime.UtcNow.AddDays(-19),
                        UpdatedAt = DateTime.UtcNow.AddDays(-19)
                    },
                    new Employee
                    {
                        FirstName = "Maria",
                        LastName = "Garcia",
                        Email = "maria.g@techcorp.com",
                        DateOfBirth = new DateTime(1991, 7, 12),
                        Mobile = "+1-555-0103",
                        CompanyId = company1.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-27),
                        UpdatedAt = DateTime.UtcNow.AddDays(-27)
                    }
                };

                await context.Employees.AddRangeAsync(employees);
                Console.WriteLine("✅ Seeded 5 Employees");
            }
        }

        private static async Task SeedProjects(ApplicationDbContext context)
        {
            if (!await context.Projects.AnyAsync())
            {
                var companies = await context.Companies.ToListAsync();
                var company1 = companies[0]; // TechCorp
                var company2 = companies[1]; // InnovateSoft
                var company3 = companies[2]; // GlobalSystems

                var projects = new List<Project>
                {
                    new Project
                    {
                        Name = "E-Commerce Platform",
                        Description = "Build a modern e-commerce platform with AI recommendations",
                        CompanyId = company1.Id,
                        DueDate = DateTime.UtcNow.AddDays(30),
                        CreatedAt = DateTime.UtcNow.AddDays(-20),
                        UpdatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new Project
                    {
                        Name = "Mobile Banking App",
                        Description = "Develop a secure mobile banking application",
                        CompanyId = company1.Id,
                        DueDate = DateTime.UtcNow.AddDays(45),
                        CreatedAt = DateTime.UtcNow.AddDays(-15),
                        UpdatedAt = DateTime.UtcNow.AddDays(-15)
                    },
                    new Project
                    {
                        Name = "CRM System",
                        Description = "Customer Relationship Management system for sales team",
                        CompanyId = company2.Id,
                        DueDate = DateTime.UtcNow.AddDays(60),
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UpdatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Project
                    {
                        Name = "Inventory Management",
                        Description = "Track and manage company inventory",
                        CompanyId = company3.Id,
                        DueDate = DateTime.UtcNow.AddDays(90),
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-5)
                    }
                };

                await context.Projects.AddRangeAsync(projects);
                Console.WriteLine("✅ Seeded 4 Projects");
            }
        }

        private static async Task SeedTasks(ApplicationDbContext context)
        {
            if (!await context.Tasks.AnyAsync())
            {
                var projects = await context.Projects.ToListAsync();
                var employees = await context.Employees.ToListAsync();

                var project1 = projects[0]; // E-Commerce Platform
                var project2 = projects[1]; // Mobile Banking App
                var project3 = projects[2]; // CRM System

                var employee1 = employees[0]; // John Smith
                var employee2 = employees[1]; // Sarah Johnson
                var employee3 = employees[2]; // David Brown
                var employee4 = employees[3]; // Yuki Tanaka

                var tasks = new List<TaskRecord>
                {
                    new TaskRecord
                    {
                        Name = "Design Database Schema",
                        Description = "Create ER diagram and database tables for e-commerce",
                        ProjectId = project1.Id,
                        EmployeeId = employee1.Id,
                        DueDate = DateTime.UtcNow.AddDays(5),
                        CreatedAt = DateTime.UtcNow.AddDays(-18),
                        UpdatedAt = DateTime.UtcNow.AddDays(-18)
                    },
                    new TaskRecord
                    {
                        Name = "Implement User Authentication",
                        Description = "Add login, registration, and password reset functionality",
                        ProjectId = project1.Id,
                        EmployeeId = employee2.Id,
                        DueDate = DateTime.UtcNow.AddDays(10),
                        CreatedAt = DateTime.UtcNow.AddDays(-17),
                        UpdatedAt = DateTime.UtcNow.AddDays(-17)
                    },
                    new TaskRecord
                    {
                        Name = "Create Product API",
                        Description = "Develop RESTful APIs for product management",
                        ProjectId = project1.Id,
                        EmployeeId = employee1.Id,
                        DueDate = DateTime.UtcNow.AddDays(15),
                        CreatedAt = DateTime.UtcNow.AddDays(-16),
                        UpdatedAt = DateTime.UtcNow.AddDays(-16)
                    },
                    new TaskRecord
                    {
                        Name = "Security Audit",
                        Description = "Perform security assessment and fix vulnerabilities",
                        ProjectId = project2.Id,
                        EmployeeId = employee1.Id,
                        DueDate = DateTime.UtcNow.AddDays(20),
                        CreatedAt = DateTime.UtcNow.AddDays(-12),
                        UpdatedAt = DateTime.UtcNow.AddDays(-12)
                    },
                    new TaskRecord
                    {
                        Name = "Design UI Mockups",
                        Description = "Create wireframes and mockups for CRM dashboard",
                        ProjectId = project3.Id,
                        EmployeeId = employee3.Id,
                        DueDate = DateTime.UtcNow.AddDays(25),
                        CreatedAt = DateTime.UtcNow.AddDays(-8),
                        UpdatedAt = DateTime.UtcNow.AddDays(-8)
                    },
                    new TaskRecord
                    {
                        Name = "Setup Development Environment",
                        Description = "Configure development servers and tools",
                        ProjectId = project1.Id,
                        DueDate = DateTime.UtcNow.AddDays(3),
                        CreatedAt = DateTime.UtcNow.AddDays(-19),
                        UpdatedAt = DateTime.UtcNow.AddDays(-19)
                    }
                };

                await context.Tasks.AddRangeAsync(tasks);
                Console.WriteLine("✅ Seeded 6 Tasks");
            }
        }

        private static async Task SeedProjectEmployees(ApplicationDbContext context)
        {
            if (!await context.ProjectEmployees.AnyAsync())
            {
                var projects = await context.Projects.ToListAsync();
                var employees = await context.Employees.ToListAsync();

                var project1 = projects[0]; // E-Commerce Platform
                var project2 = projects[1]; // Mobile Banking App
                var project3 = projects[2]; // CRM System

                var employee1 = employees[0]; // John Smith
                var employee2 = employees[1]; // Sarah Johnson
                var employee3 = employees[2]; // David Brown
                var employee4 = employees[3]; // Yuki Tanaka
                var employee5 = employees[4]; // Maria Garcia

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
                        ProjectId = project1.Id,
                        EmployeeId = employee5.Id,
                        AssignedDate = DateTime.UtcNow.AddDays(-16)
                    },
                    new ProjectEmployee
                    {
                        ProjectId = project2.Id,
                        EmployeeId = employee1.Id,
                        AssignedDate = DateTime.UtcNow.AddDays(-12)
                    },
                    new ProjectEmployee
                    {
                        ProjectId = project2.Id,
                        EmployeeId = employee4.Id,
                        AssignedDate = DateTime.UtcNow.AddDays(-11)
                    },
                    new ProjectEmployee
                    {
                        ProjectId = project3.Id,
                        EmployeeId = employee3.Id,
                        AssignedDate = DateTime.UtcNow.AddDays(-8)
                    }
                };

                await context.ProjectEmployees.AddRangeAsync(projectEmployees);
                Console.WriteLine("✅ Seeded 6 Project-Employee Assignments");
            }
        }
    }
}