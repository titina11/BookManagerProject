namespace BookManager.ViewModels.Book
{
    public class BookViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Author { get; set; } = null!; 

        public string Publisher { get; set; } = null!;

        public string Genre { get; set; } = null!;  

        public string? ImageUrl { get; set; }
    }
}