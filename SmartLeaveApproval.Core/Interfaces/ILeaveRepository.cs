using SmartLeaveApproval.Core.Entities;

namespace SmartLeaveApproval.Core.Interfaces;

public interface ILeaveRepository
{
    Task<LeaveRequest> CreateAsync(LeaveRequest request);
    Task<LeaveRequest?> GetByIdAsync(int id);
    Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<LeaveRequest>> GetAllAsync();
    Task<int> GetPendingCountAsync(int employeeId);
    Task<bool> HasOverlappingLeaveAsync(int employeeId, DateTime start, DateTime end);
    Task UpdateAsync(LeaveRequest request);
    Task AddAuditLogAsync(LeaveAuditLog log);
}