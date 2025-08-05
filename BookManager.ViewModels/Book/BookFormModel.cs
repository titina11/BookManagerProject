using BookManager.ViewModels.Publisher;

namespace BookManager.ViewModels.Book
{
    public class BookFormModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public Guid AuthorId { get; set; }
        public Guid GenreId { get; set; }
        public Guid PublisherId { get; set; }

        public List<AuthorDropdownViewModel> Authors { get; set; } = new();
        public List<GenreDropdownViewModel> Genres { get; set; } = new();
        public List<PublisherDropdownViewModel> Publishers { get; set; } = new();
    }
}