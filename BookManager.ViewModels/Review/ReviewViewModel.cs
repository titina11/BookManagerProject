namespace BookManager.ViewModels.Review
{
    public class ReviewViewModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; } = null!;

        public int Rating { get; set; }

        public string UserEmail { get; set; } = null!;
    }
}