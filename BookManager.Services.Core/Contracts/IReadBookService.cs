using BookManager.ViewModels.Read;
using BookManager.ViewModels.UserBooks;

public interface IReadBookService
{
    Task<IEnumerable<ReadBookViewModel>> GetByUserAsync(string userId);
    Task AddAsync(string userId, ReadBookFormModel model);
}