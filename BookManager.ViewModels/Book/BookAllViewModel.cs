
using BookManager.ViewModels.Author;
using BookManager.ViewModels.Genre;
using BookManager.ViewModels.Publisher;

namespace BookManager.ViewModels.Book
{
    public class BookAllViewModel
    {
        public List<BookViewModel> Books { get; set; } = new();

        public string? SearchTitle { get; set; }
        public Guid? SelectedAuthorId { get; set; }
        public Guid? SelectedGenreId { get; set; }
        public Guid? SelectedPublisherId { get; set; }

        public List<AuthorListViewModel> Authors { get; set; } = new();
        public List<GenreViewModel> Genres { get; set; } = new();
        public List<PublisherViewModel> Publishers { get; set; } = new();

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}