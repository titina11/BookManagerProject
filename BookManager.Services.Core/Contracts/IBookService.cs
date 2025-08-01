using BookManager.ViewModels.Book;

public interface IBookService
{
    Task<IEnumerable<BookViewModel>> GetAllAsync();
    Task<BookViewModel?> GetByIdAsync(Guid id);

    Task<CreateBookViewModel> GetCreateModelAsync();
    Task<EditBookViewModel?> GetEditModelAsync(Guid id);

    Task CreateAsync(CreateBookViewModel model);
    Task EditAsync(Guid id, EditBookViewModel model);

    Task DeleteAsync(Guid id);
}