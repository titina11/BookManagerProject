using BookManager.Data.Models;
using BookManager.ViewModels.Admin;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookManager.Services.Core.Contracts;

namespace BookManager.Services.Core
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserWithRoleViewModel>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserWithRoleViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    IsAdmin = roles.Contains("Admin")
                });
            }

            return result;
        }

        public async Task<bool> ToggleAdminRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            else
                await _userManager.AddToRoleAsync(user, "Admin");

            return true;
        }
    }
}