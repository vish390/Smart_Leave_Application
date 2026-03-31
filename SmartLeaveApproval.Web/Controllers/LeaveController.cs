using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLeaveApproval.Core.DTOs;
using SmartLeaveApproval.Core.Interfaces;
using System.Security.Claims;

namespace SmartLeaveApproval.Web.Controllers;

[Authorize(Roles = "Employee")]
public class LeaveController : Controller
{
    private readonly ILeaveService _leaveService;
    private readonly IValidator<ApplyLeaveDto> _validator;

    public LeaveController(ILeaveService leaveService, IValidator<ApplyLeaveDto> validator)
    {
        _leaveService = leaveService;
        _validator = validator;
    }

    private int GetEmployeeId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var leaves = await _leaveService.GetLeavesByEmployeeAsync(GetEmployeeId());
        return View(leaves);
    }

    [HttpGet] public IActionResult Apply() => View();

    [HttpPost]
    public async Task<IActionResult> Apply(ApplyLeaveDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            foreach (var err in validation.Errors)
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            return View(dto);
        }

        var (success, message, _) = await _leaveService.ApplyLeaveAsync(GetEmployeeId(), dto);

        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction("Index");
    }
}