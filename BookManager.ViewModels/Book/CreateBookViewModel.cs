namespace BookManager.ViewModels.Book
{
    public class CreateBookViewModel
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public Guid AuthorId { get; set; }

        public Guid GenreId { get; set; }

        public Guid PublisherId { get; set; }

        public string? ImageUrl { get; set; }

        public IEnumerable<AuthorDropdownViewModel> Authors { get; set; } = new List<AuthorDropdownViewModel>();
        public IEnumerable<GenreDropdownViewModel> Genres { get; set; } = new List<GenreDropdownViewModel>();
        public IEnumerable<PublisherDropdownViewModel> Publishers { get; set; } = new List<PublisherDropdownViewModel>();
    }

    public class AuthorDropdownViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class GenreDropdownViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class PublisherDropdownViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}