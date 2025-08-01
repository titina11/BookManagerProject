using BookManager.ViewModels.Book;

namespace BookManager.ViewModels.Author;

public class AuthorListViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<BookShortViewModel> Books { get; set; } = new List<BookShortViewModel>();
}