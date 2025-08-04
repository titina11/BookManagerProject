using BookManager.Services.Core;
using BookManager.Services.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _adminService.GetAllUsersAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> RoleEdit()
    {
        var users = await _adminService.GetAllUsersAsync();
        return View("RoleEdit", users); 
    }

    [HttpPost]
    public async Task<IActionResult> ToggleAdminRole(string userId)
    {
        await _adminService.ToggleAdminRoleAsync(userId);
        return RedirectToAction(nameof(RoleEdit));
    }
}