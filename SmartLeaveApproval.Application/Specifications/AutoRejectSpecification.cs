using SmartLeaveApproval.Core.Entities;

namespace SmartLeaveApproval.Application.Specifications;

public class AutoRejectSpecification : ILeaveSpecification
{
    public string Reason => "Auto-rejected: employee has more than 3 pending leave requests.";

    public bool IsSatisfiedBy(LeaveRequest request, int pendingCount)
    {
        return pendingCount > 3;
    }
}