    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using ProjectTaskManager.Entities;
    using ProjectTaskManager.Services.Interfaces;
    using System.Reflection.Emit;

    namespace ProjectTaskManager.Data.Context
    {
        public class ApplicationDbContext : IdentityDbContext<User>
        {
            private readonly ICurrentUserService _currentUserService;

            public ApplicationDbContext(
                DbContextOptions<ApplicationDbContext> options,
                ICurrentUserService currentUserService) : base(options)
            {
                _currentUserService = currentUserService;
            }

            public DbSet<Company> Companies { get; set; }
            public DbSet<Project> Projects { get; set; }
            public DbSet<TaskRecord> Tasks { get; set; }
            public DbSet<ProjectEmployee> ProjectEmployees { get; set; }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);

                // Unique company name
                builder.Entity<Company>()
                    .HasIndex(c => c.Name)
                    .IsUnique();

                // Company-Project
                builder.Entity<Project>()
                    .HasOne(p => p.Company)
                    .WithMany(c => c.Projects)
                    .HasForeignKey(p => p.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Company-User
                builder.Entity<User>()
                    .HasOne(u => u.Company)
                    .WithMany(c => c.Users)
                    .HasForeignKey(u => u.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Project-Task
                builder.Entity<TaskRecord>()
                    .HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                // User-Task
                builder.Entity<TaskRecord>()
                    .HasOne(t => t.User)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Many-to-many User-Project
                builder.Entity<ProjectEmployee>()
                    .HasKey(pe => pe.Id);

                builder.Entity<ProjectEmployee>()
                    .HasOne(pe => pe.Project)
                    .WithMany(p => p.ProjectEmployees)
                    .HasForeignKey(pe => pe.ProjectId);

                builder.Entity<ProjectEmployee>()
                    .HasOne(pe => pe.User)
                    .WithMany(u => u.ProjectEmployees)
                    .HasForeignKey(pe => pe.UserId);

                // Global query filter for soft delete
                builder.Entity<Company>().HasQueryFilter(e => e.InactiveDate == null);
                builder.Entity<Project>().HasQueryFilter(e => e.InactiveDate == null);
                builder.Entity<TaskRecord>().HasQueryFilter(e => e.InactiveDate == null);
                builder.Entity<ProjectEmployee>().HasQueryFilter(e => e.InactiveDate == null);
            }

            public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                var userId = _currentUserService.GetCurrentUserId();

                foreach (var entry in ChangeTracker.Entries<BaseEntity>())
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedAt = DateTime.UtcNow;
                            entry.Entity.CreatedBy = userId;
                            break;
                        case EntityState.Modified:
                            entry.Entity.UpdatedAt = DateTime.UtcNow;
                            entry.Entity.UpdatedBy = userId;
                            break;
                    }
                }

                return await base.SaveChangesAsync(cancellationToken);
            }
        }
    }