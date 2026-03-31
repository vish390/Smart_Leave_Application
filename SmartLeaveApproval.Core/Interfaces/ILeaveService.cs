using SmartLeaveApproval.Core.DTOs;

namespace SmartLeaveApproval.Core.Interfaces;

public interface ILeaveService
{
    Task<(bool success, string message, int leaveId)> ApplyLeaveAsync(int employeeId, ApplyLeaveDto dto);
    Task<(bool success, string message)> AdminOverrideAsync(AdminOverrideDto dto, string adminName);
    Task<IEnumerable<LeaveRequestViewModel>> GetLeavesByEmployeeAsync(int employeeId);
    Task<IEnumerable<LeaveRequestViewModel>> GetAllLeavesAsync();
}