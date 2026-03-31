using Microsoft.EntityFrameworkCore;
using SmartLeaveApproval.Core.Entities;
using SmartLeaveApproval.Core.Interfaces;
using SmartLeaveApproval.Infrastructure.Data;

namespace SmartLeaveApproval.Infrastructure.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly AppDbContext _db;

    public LeaveRepository(AppDbContext db) => _db = db;

    public async Task<LeaveRequest> CreateAsync(LeaveRequest request)
    {
        _db.LeaveRequests.Add(request);
        await _db.SaveChangesAsync();
        return request;
    }

    public async Task<LeaveRequest?> GetByIdAsync(int id) =>
        await _db.LeaveRequests
            .Include(x => x.Employee)
            .Include(x => x.AuditLogs)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId) =>
        await _db.LeaveRequests
            .Include(x => x.AuditLogs)
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.AppliedOn)
            .ToListAsync();

    public async Task<IEnumerable<LeaveRequest>> GetAllAsync() =>
        await _db.LeaveRequests
            .Include(x => x.Employee)
            .Include(x => x.AuditLogs)
            .OrderByDescending(x => x.AppliedOn)
            .ToListAsync();

    public async Task<int> GetPendingCountAsync(int employeeId) =>
        await _db.LeaveRequests
            .CountAsync(x => x.EmployeeId == employeeId && x.Status == "Pending");

    public async Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime start, DateTime end) =>
        await _db.LeaveRequests
            .AnyAsync(x => x.EmployeeId == employeeId
                        && x.Status != "Rejected"
                        && x.StartDate <= end
                        && x.EndDate >= start);

    public async Task UpdateAsync(LeaveRequest request)
    {
        _db.LeaveRequests.Update(request);
        await _db.SaveChangesAsync();
    }

    public async Task AddAuditLogAsync(LeaveAuditLog log)
    {
        _db.LeaveAuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}