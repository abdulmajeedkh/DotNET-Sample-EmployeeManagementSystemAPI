using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Entities.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
            var tenantId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("tid");

            foreach (var entry in ChangeTracker.Entries<IEntity<Guid>>())
            {
                if (entry.State == EntityState.Added && entry.Entity is EntityBase<Guid> baseEntity)
                {
                    if (string.IsNullOrWhiteSpace(baseEntity.UserId))
                    {
                        baseEntity.UserId = userId;
                    }
                    if (!baseEntity.TenantId.HasValue)
                    {
                        baseEntity.TenantId = new Guid(tenantId);
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Attendance> AttendanceRecords => Set<Attendance>();
        public DbSet<Leave> LeaveRequests => Set<Leave>();
        public DbSet<Payroll> Payrolls => Set<Payroll>();
        public DbSet<Training> Trainings => Set<Training>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>()
           .HasMany(t => t.Departments)
           .WithOne(d => d.Tenant)
           .HasForeignKey(d => d.TenantId);

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId);
            modelBuilder.Entity<Attendance>()
       .HasOne(a => a.Employee)
       .WithMany()
       .HasForeignKey(a => a.EmployeeId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Tenant)
                .WithMany()
                .HasForeignKey(a => a.TenantId);
            // Leave relationships
            modelBuilder.Entity<Leave>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmployeeId);

            modelBuilder.Entity<Leave>()
                .HasOne(l => l.Tenant)
                .WithMany()
                .HasForeignKey(l => l.TenantId);

            // Payroll → Employee, Tenant
            modelBuilder.Entity<Payroll>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId);

            modelBuilder.Entity<Payroll>()
                .HasOne(p => p.Tenant)
                .WithMany()
                .HasForeignKey(p => p.TenantId);

            modelBuilder.Entity<Training>()
     .HasOne(t => t.Employee)
     .WithMany()
     .HasForeignKey(t => t.EmployeeId);

            modelBuilder.Entity<Training>()
                .HasOne(t => t.Tenant)
                .WithMany()
                .HasForeignKey(t => t.TenantId);

            modelBuilder.Entity<Employee>()
      .HasOne(e => e.Tenant)
      .WithMany(t => t.Employees)
      .HasForeignKey(e => e.TenantId);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Tenant)
                .WithMany(t => t.Departments)
                .HasForeignKey(d => d.TenantId);

            modelBuilder.Entity<Tenant>().HasIndex(t => t.CompanyName).IsUnique();
        }
    }
}
