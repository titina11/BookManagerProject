namespace BookManager.ViewModels.Review
{
    public class ReviewDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; } = null!;

        public int Rating { get; set; }

        public string UserEmail { get; set; } = null!;

        public Guid BookId { get; set; }
    }
}