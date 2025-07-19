using BookManager.Data.Models;
using Microsoft.AspNetCore.Identity;

public class UserBook
{
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}
