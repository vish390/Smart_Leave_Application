using Microsoft.Extensions.Logging;
using SmartLeaveApproval.Core.DTOs;
using SmartLeaveApproval.Core.Entities;
using SmartLeaveApproval.Core.Interfaces;

namespace SmartLeaveApproval.Application.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leaveRepo;
    private readonly LeaveEvaluator _evaluator;
    private readonly ILogger<LeaveService> _logger;

    public LeaveService(ILeaveRepository leaveRepo, LeaveEvaluator evaluator, ILogger<LeaveService> logger)
    {
        _leaveRepo = leaveRepo;
        _evaluator = evaluator;
        _logger = logger;
    }

    public async Task<(bool success, string message, int leaveId)> ApplyLeaveAsync(int employeeId, ApplyLeaveDto dto)
    {
        // 1. Overlap check
        var hasOverlap = await _leaveRepo.HasOverlappingLeaveAsync(employeeId, dto.StartDate, dto.EndDate);
        if (hasOverlap)
        {
            _logger.LogWarning("Employee {Id} has overlapping leave dates.", employeeId);
            return (false, "You already have a leave request for these dates.", 0);
        }

        // 2. Create request
        var request = new LeaveRequest
        {
            EmployeeId = employeeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            LeaveType = dto.LeaveType,
            Status = "Pending",
            AppliedOn = DateTime.UtcNow
        };

        await _leaveRepo.CreateAsync(request);

        // 3. Auto-evaluate
        var pendingCount = await _leaveRepo.GetPendingCountAsync(employeeId);
        var (newStatus, reason) = _evaluator.Evaluate(request, pendingCount);

        if (newStatus != "Pending")
        {
            var oldStatus = request.Status;
            request.Status = newStatus;
            await _leaveRepo.UpdateAsync(request);

            await _leaveRepo.AddAuditLogAsync(new LeaveAuditLog
            {
                LeaveRequestId = request.Id,
                ChangedBy = "System",
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Comment = reason,
                ChangedAt = DateTime.UtcNow
            });
        }

        _logger.LogInformation("Leave {Id} created with status {Status}.", request.Id, request.Status);
        return (true, $"Leave applied successfully. Status: {request.Status}", request.Id);
    }

    public async Task<(bool success, string message)> AdminOverrideAsync(AdminOverrideDto dto, string adminName)
    {
        var request = await _leaveRepo.GetByIdAsync(dto.LeaveRequestId);
        if (request == null)
            return (false, "Leave request not found.");

        var oldStatus = request.Status;
        request.Status = dto.NewStatus;
        await _leaveRepo.UpdateAsync(request);

        await _leaveRepo.AddAuditLogAsync(new LeaveAuditLog
        {
            LeaveRequestId = request.Id,
            ChangedBy = adminName,
            OldStatus = oldStatus,
            NewStatus = dto.NewStatus,
            Comment = dto.Comment,
            ChangedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Admin {Admin} overrode leave {Id} to {Status}.", adminName, dto.LeaveRequestId, dto.NewStatus);
        return (true, "Leave status updated successfully.");
    }

    public async Task<IEnumerable<LeaveRequestViewModel>> GetLeavesByEmployeeAsync(int employeeId)
    {
        var leaves = await _leaveRepo.GetByEmployeeIdAsync(employeeId);
        return MapToViewModel(leaves);
    }

    public async Task<IEnumerable<LeaveRequestViewModel>> GetAllLeavesAsync()
    {
        var leaves = await _leaveRepo.GetAllAsync();
        return MapToViewModel(leaves);
    }

    private static IEnumerable<LeaveRequestViewModel> MapToViewModel(IEnumerable<Core.Entities.LeaveRequest> leaves)
    {
        return leaves.Select(l => new LeaveRequestViewModel
        {
            Id = l.Id,
            EmployeeName = l.Employee?.FullName ?? "Unknown",
            StartDate = l.StartDate,
            EndDate = l.EndDate,
            Reason = l.Reason,
            LeaveType = l.LeaveType,
            Status = l.Status,
            AppliedOn = l.AppliedOn,
            AuditLogs = l.AuditLogs.Select(a => new AuditLogViewModel
            {
                ChangedBy = a.ChangedBy,
                OldStatus = a.OldStatus,
                NewStatus = a.NewStatus,
                Comment = a.Comment,
                ChangedAt = a.ChangedAt
            }).ToList()
        });
    }
}