using FluentValidation;
using SmartLeaveApproval.Core.DTOs;

namespace SmartLeaveApproval.Application.Validators;

public class ApplyLeaveValidator : AbstractValidator<ApplyLeaveDto>
{
    public ApplyLeaveValidator()
    {
        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be after start date.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(500);

        RuleFor(x => x.LeaveType)
            .Must(x => new[] { "Sick", "Casual", "Annual" }.Contains(x))
            .WithMessage("Leave type must be Sick, Casual, or Annual.");
    }
}