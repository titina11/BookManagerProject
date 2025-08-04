using BookManager.Data.Models;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var userRoles = new List<(ApplicationUser User, bool IsAdmin)>();

            foreach (var user in users)
            {
                bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                userRoles.Add((user, isAdmin));
            }

            return View(userRoles);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAdminRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}