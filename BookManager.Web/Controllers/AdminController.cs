using BookManager.Services.Core;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.Web.Controllers;

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
        try
        {
            var users = await _adminService.GetAllUsersAsync();
            return View("RoleEdit", users);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Грешка при зареждане на потребителите: {ex.Message}";
            return View("RoleEdit", new List<UserWithRoleViewModel>()); 
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleAdminRole(string userId)
    {
        try
        {
            var success = await _adminService.ToggleAdminRoleAsync(userId);

            if (!success)
            {
                TempData["Error"] = "Неуспешна промяна на ролята. Потребителят не е намерен или е невалиден.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Грешка при промяна на ролята: {ex.Message}";
        }

        return RedirectToAction(nameof(RoleEdit));
    }
}