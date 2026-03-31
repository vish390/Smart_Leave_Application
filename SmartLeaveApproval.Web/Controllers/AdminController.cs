using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLeaveApproval.Core.DTOs;
using SmartLeaveApproval.Core.Interfaces;
using System.Security.Claims;

namespace SmartLeaveApproval.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ILeaveService _leaveService;

    public AdminController(ILeaveService leaveService) => _leaveService = leaveService;

    public async Task<IActionResult> Index()
    {
        var leaves = await _leaveService.GetAllLeavesAsync();
        return View(leaves);
    }

    [HttpPost]
    public async Task<IActionResult> Override(AdminOverrideDto dto)
    {
        var adminName = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";
        var (success, message) = await _leaveService.AdminOverrideAsync(dto, adminName);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction("Index");
    }
}