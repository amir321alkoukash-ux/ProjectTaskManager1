using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProjectTaskManager.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Company Configuration
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();
                entity.HasIndex(c => c.Email).IsUnique();
            });

            // Project Configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasOne(p => p.Company)
                    .WithMany(c => c.Projects)
                    .HasForeignKey(p => p.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Employee Configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasOne(e => e.Company)
                    .WithMany(c => c.Employees)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Task Configuration
            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Employee)
                    .WithMany(e => e.Tasks)
                    .HasForeignKey(t => t.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ProjectEmployee (Many-to-Many) Configuration
            modelBuilder.Entity<ProjectEmployee>(entity =>
            {
                entity.HasOne(pe => pe.Project)
                    .WithMany(p => p.ProjectEmployees)
                    .HasForeignKey(pe => pe.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pe => pe.Employee)
                    .WithMany(e => e.ProjectEmployees)
                    .HasForeignKey(pe => pe.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Ensure unique combination of Project and Employee
                entity.HasIndex(pe => new { pe.ProjectId, pe.EmployeeId }).IsUnique();
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}