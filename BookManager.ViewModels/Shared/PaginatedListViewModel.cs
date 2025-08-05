using BookManager.ViewModels.Book;
using BookManager.ViewModels.Publisher;

namespace BookManager.ViewModels.Shared
{
    public class PaginatedListViewModel<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public List<T> Books { get; set; } = new();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public string? SearchTitle { get; set; }
        public Guid? SelectedAuthorId { get; set; }
        public Guid? SelectedGenreId { get; set; }
        public Guid? SelectedPublisherId { get; set; }

        public List<AuthorDropdownViewModel> Authors { get; set; } = new();
        public List<GenreDropdownViewModel> Genres { get; set; } = new();
        public List<PublisherDropdownViewModel> Publishers { get; set; } = new();
    }
}