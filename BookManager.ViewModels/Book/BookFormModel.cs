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

        public IEnumerable<AuthorDropdownViewModel> Authors { get; set; } = new List<AuthorDropdownViewModel>();
        public IEnumerable<GenreDropdownViewModel> Genres { get; set; } = new List<GenreDropdownViewModel>();
        public IEnumerable<PublisherDropdownViewModel> Publishers { get; set; } = new List<PublisherDropdownViewModel>();
    }
}