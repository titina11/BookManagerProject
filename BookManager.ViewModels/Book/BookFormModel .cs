namespace BookManager.ViewModels.Book
{
    public class BookFormModel
    {
        public string Title { get; set; } = null!;

        public int AuthorId { get; set; }

        public int PublisherId { get; set; }

        public int GenreId { get; set; }

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }
}