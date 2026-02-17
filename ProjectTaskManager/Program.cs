using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Data.Repositories.Implementations;
using ProjectTaskManager.Services.Interfaces;
using ProjectTaskManager.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Logging;
using ProjectTaskManager.Data;

namespace ProjectTaskManager
{
    public class Program  
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // DbContext
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Identity
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddRoles<IdentityRole<int>>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.LoginPath = "/api/user/login";
                options.LogoutPath = "/api/user/logout";
                options.SlidingExpiration = true;
            });

            // Repositories
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<ILoggerManager, LoggerManager>();


            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Fixed test endpoints
            app.MapGet("/test", () => "API is running!");

            // Add these test endpoints
            app.MapGet("/test/companies", async (ApplicationDbContext db) =>
                await db.Companies.ToListAsync());

            app.MapGet("/test/employees", async (ApplicationDbContext db) =>
                await db.Employees.ToListAsync());

            app.MapGet("/test/projects", async (ApplicationDbContext db) =>
                await db.Projects.ToListAsync());




            app.MapGet("/test-db", async (ApplicationDbContext db) =>
            {
 
            }
            );

            app.Run();
        }
    }
}