namespace SmartLeaveApproval.Core.Entities;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty; // Sick, Casual, Annual
    public string Status { get; set; } = "Pending";       // Pending, Approved, Rejected
    public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

    // Navigation
    public Employee? Employee { get; set; }
    public ICollection<LeaveAuditLog> AuditLogs { get; set; } = new List<LeaveAuditLog>();
}