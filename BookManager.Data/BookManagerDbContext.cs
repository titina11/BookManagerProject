
using System.Reflection;
using BookManager.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookManager.Web.Areas.Identity.Data;

namespace BookManager.Web.Areas.Identity.Data;

public class BookManagerDbContext : IdentityDbContext<ApplicationUser>
{
    public BookManagerDbContext(DbContextOptions<BookManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books => Set<Book>();
    public virtual DbSet<Author> Authors => Set<Author>();
    public virtual DbSet<Genre> Genres => Set<Genre>();
    public virtual DbSet<Publisher> Publishers => Set<Publisher>();
    public virtual DbSet<Review> Reviews => Set<Review>();

    public virtual DbSet<UserBook> UserBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new BookConfiguration());
        builder.ApplyConfiguration(new AuthorConfiguration());
        builder.ApplyConfiguration(new GenreConfiguration());
        builder.ApplyConfiguration(new PublisherConfiguration());
        builder.ApplyConfiguration(new UserBookConfiguration());
        builder.ApplyConfiguration(new ApplicationUserConfiguration());
    }
}


