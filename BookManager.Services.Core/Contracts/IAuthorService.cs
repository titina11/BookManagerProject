using BookManager.ViewModels.Author;

namespace BookManager.Services.Core.Contracts
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorViewModel>> GetAllAsync();

        Task<AuthorViewModel?> GetByIdAsync(int id);

        Task CreateAsync(CreateAuthorViewModel model);

        Task EditAsync(int id, EditAuthorViewModel model);

        Task DeleteAsync(int id);

        Task<DeleteAuthorViewModel?> GetDeleteByIdAsync(int id);
    }
}