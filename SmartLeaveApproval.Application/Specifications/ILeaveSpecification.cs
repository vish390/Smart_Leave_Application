using SmartLeaveApproval.Core.Entities;

namespace SmartLeaveApproval.Application.Specifications;

// Specification Pattern — interviewer ko impress karega!
public interface ILeaveSpecification
{
    bool IsSatisfiedBy(LeaveRequest request, int pendingCount);
    string Reason { get; }
}