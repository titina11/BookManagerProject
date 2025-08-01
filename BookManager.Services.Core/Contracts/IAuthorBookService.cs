using BookManager.ViewModels.Authors;

namespace BookManager.Services.Core
{
    public interface IAuthorBookService
    {
        Task<AddBookToAuthorViewModel> GetAddBookModelAsync(Guid authorId);
        Task AddBookToAuthorAsync(AddBookToAuthorViewModel model);
    }
}