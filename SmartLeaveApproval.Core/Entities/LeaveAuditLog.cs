namespace SmartLeaveApproval.Core.Entities;

public class LeaveAuditLog
{
    public int Id { get; set; }
    public int LeaveRequestId { get; set; }
    public string ChangedBy { get; set; } = string.Empty;  // Admin/System
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public LeaveRequest? LeaveRequest { get; set; }
}