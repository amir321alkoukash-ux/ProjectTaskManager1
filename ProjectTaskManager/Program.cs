using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using ProjectTaskManager.Data.Context;
using ProjectTaskManager.Data.Repositories.Implementations;
using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Middlewares;
using ProjectTaskManager.Services.Implementations;
using ProjectTaskManager.Services.Interfaces;
using ProjectTaskManager.Services.Logging;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();

    // Add DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Register Repositories
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    // Register Services
    builder.Services.AddScoped<ICompanyService, CompanyService>();
    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<IEmployeeService, EmployeeService>();
    builder.Services.AddScoped<ITaskService, TaskService>();
    builder.Services.AddSingleton<ILoggerManager, LoggerManager>(); 

    // Add Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configure NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Add Middleware
    app.UseMiddleware<LoggingMiddleware>();

    app.UseAuthorization();

    app.MapControllers();

    // Apply migrations
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}