using BookManager.ViewModels.Admin;

namespace BookManager.Services.Core.Contracts
{
    public interface IAdminService
    {
        Task<List<UserWithRoleViewModel>> GetAllUsersAsync();
        Task<bool> ToggleAdminRoleAsync(string userId);
    }
}