using BookManager.ViewModels.Book;

public interface IBookService
{
    Task<IEnumerable<BookViewModel>> GetAllAsync();
    Task<BookViewModel?> GetByIdAsync(int id);

    Task<CreateBookViewModel> GetCreateModelAsync();
    Task<EditBookViewModel?> GetEditModelAsync(int id);

    Task CreateAsync(CreateBookViewModel model);
    Task EditAsync(int id, EditBookViewModel model);

    Task DeleteAsync(int id);
}