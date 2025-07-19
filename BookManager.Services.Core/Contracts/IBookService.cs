
using BookManager.ViewModels;

namespace BookManager.Services.Core.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookViewModel>> GetAllAsync();
        Task<BookViewModel?> GetByIdAsync(int id);
        Task CreateAsync(BookViewModel model);
        Task EditAsync(int id, BookViewModel model);
        Task DeleteAsync(int id);
    }
}