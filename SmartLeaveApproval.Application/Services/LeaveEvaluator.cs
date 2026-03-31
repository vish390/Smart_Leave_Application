using SmartLeaveApproval.Application.Specifications;
using SmartLeaveApproval.Core.Entities;

namespace SmartLeaveApproval.Application.Services;

public class LeaveEvaluator
{
    private readonly AutoApproveSpecification _approveSpec = new();
    private readonly AutoRejectSpecification _rejectSpec = new();

    public (string status, string reason) Evaluate(LeaveRequest request, int pendingCount)
    {
        if (_rejectSpec.IsSatisfiedBy(request, pendingCount))
            return ("Rejected", _rejectSpec.Reason);

        if (_approveSpec.IsSatisfiedBy(request, pendingCount))
            return ("Approved", _approveSpec.Reason);

        return ("Pending", "Manual review required.");
    }
}