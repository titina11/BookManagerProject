using BookManager.ViewModels.Book;
using BookManager.ViewModels.Publisher;
using BookManager.ViewModels.Shared;

public interface IBookService
{
    Task<IEnumerable<BookViewModel>> GetAllAsync();


    Task<BookViewModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<BookViewModel>> GetLatestAsync(int count);

    Task<EditBookViewModel?> GetEditModelAsync(Guid id);
    Task EditAsync(Guid id, EditBookViewModel model, string currentUserId, bool isAdmin);
    Task<CreateBookViewModel> GetCreateModelAsync();
    Task CreateAsync(CreateBookViewModel model, string createdByUserId);
    Task<BookViewModel?> GetDetailsByIdAsync(Guid id);

    Task<BookAllViewModel> GetFilteredAsync(string? title, Guid? authorId, Guid? genreId, Guid? publisherId, int page, int pageSize);


    Task DeleteAsync(Guid id);

    Task<List<AuthorDropdownViewModel>> GetAuthorsAsync();
    Task<List<GenreDropdownViewModel>> GetGenresAsync();
    Task<List<PublisherDropdownViewModel>> GetPublishersAsync();
}