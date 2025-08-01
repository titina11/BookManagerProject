
using BookManager.ViewModels.Author;
using BookManager.ViewModels.Authors;


public interface IAuthorService
{
    Task<IEnumerable<AuthorListViewModel>> GetAllAsync();

    Task<AddBookToAuthorViewModel> GetAddBookModelAsync(Guid authorId);

    Task AddBookToAuthorAsync(AddBookToAuthorViewModel model);

    Task CreateAsync(CreateAuthorViewModel model);

}
