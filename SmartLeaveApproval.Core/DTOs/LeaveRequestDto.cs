namespace SmartLeaveApproval.Core.DTOs;

public class ApplyLeaveDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
}

public class AdminOverrideDto
{
    public int LeaveRequestId { get; set; }
    public string NewStatus { get; set; } = string.Empty;  // Approved / Rejected
    public string Comment { get; set; } = string.Empty;
}

public class LeaveRequestViewModel
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedOn { get; set; }
    public List<AuditLogViewModel> AuditLogs { get; set; } = new();
}

public class AuditLogViewModel
{
    public string ChangedBy { get; set; } = string.Empty;
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime ChangedAt { get; set; }
}