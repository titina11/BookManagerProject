using Microsoft.AspNetCore.Identity;

namespace BookManager.Data.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<UserBook> UserBooks { get; set; } = new HashSet<UserBook>();
}
