using Microsoft.EntityFrameworkCore;
using SmartLeaveApproval.Core.Entities;

namespace SmartLeaveApproval.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveAuditLog> LeaveAuditLogs => Set<LeaveAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Employee
        modelBuilder.Entity<Employee>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).IsRequired().HasMaxLength(200);
            e.Property(x => x.Role).HasDefaultValue("Employee");
        });

        // LeaveRequest
        modelBuilder.Entity<LeaveRequest>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasDefaultValue("Pending");
            e.HasOne(x => x.Employee)
             .WithMany(x => x.LeaveRequests)
             .HasForeignKey(x => x.EmployeeId);
        });

        // LeaveAuditLog
        modelBuilder.Entity<LeaveAuditLog>(e => {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.LeaveRequest)
             .WithMany(x => x.AuditLogs)
             .HasForeignKey(x => x.LeaveRequestId);
        });
    }
}