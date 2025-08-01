using BookManager.ViewModels.Book;

public interface IBookService
{
    Task<IEnumerable<BookViewModel>> GetAllAsync();
    Task<BookViewModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<BookViewModel>> GetLatestAsync(int count);

    Task<EditBookViewModel?> GetEditModelAsync(Guid id);
    Task EditAsync(Guid id, EditBookViewModel model);
    Task<CreateBookViewModel> GetCreateModelAsync();
    Task CreateAsync(CreateBookViewModel model);
    Task<BookViewModel?> GetDetailsByIdAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task<List<AuthorDropdownViewModel>> GetAuthorsAsync();
    Task<List<GenreDropdownViewModel>> GetGenresAsync();
    Task<List<PublisherDropdownViewModel>> GetPublishersAsync();
}