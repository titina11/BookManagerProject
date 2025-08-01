using BookManager.ViewModels.Author;

namespace BookManager.Services.Core.Contracts
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorViewModel>> GetAllAsync();

        Task<AuthorViewModel?> GetByIdAsync(Guid id);

        Task CreateAsync(CreateAuthorViewModel model);

        Task EditAsync(Guid id, EditAuthorViewModel model);

        Task DeleteAsync(Guid id);

        Task<DeleteAuthorViewModel?> GetDeleteByIdAsync(Guid id);
    }
}