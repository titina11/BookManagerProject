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
    Task DeleteAsync(Guid id);

    Task<IEnumerable<AuthorDropdownViewModel>> GetAuthorsAsync();
    Task<IEnumerable<GenreDropdownViewModel>> GetGenresAsync();
    Task<IEnumerable<PublisherDropdownViewModel>> GetPublishersAsync();
}