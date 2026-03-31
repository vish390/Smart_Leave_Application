using SmartLeaveApproval.Core.Entities;

namespace SmartLeaveApproval.Application.Specifications;

public class AutoApproveSpecification : ILeaveSpecification
{
    public string Reason => "Auto-approved: leave duration is 2 days or less.";

    public bool IsSatisfiedBy(LeaveRequest request, int pendingCount)
    {
        var duration = (request.EndDate - request.StartDate).TotalDays + 1;
        return duration <= 2;
    }
}