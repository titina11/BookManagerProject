namespace BookManager.ViewModels.UserBooks
{
    public class ReadBookViewModel
    {
        public Guid Id { get; set; } 

        public Guid BookId { get; set; }

        public string BookTitle { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Rating { get; set; }

        public int DaysRead => (EndDate - StartDate).Days;
    }
}