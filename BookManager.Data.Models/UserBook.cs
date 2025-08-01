using BookManager.Data.Models;

public class UserBook
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int Rating { get; set; } 

    public TimeSpan ReadingDuration => EndDate - StartDate;
}