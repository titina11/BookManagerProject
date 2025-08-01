namespace BookManager.ViewModels.Book
{
    public class BookFormModel
    {
        public string Title { get; set; } = null!;

        public Guid AuthorId { get; set; }

        public Guid PublisherId { get; set; }

        public Guid GenreId { get; set; }

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }
}