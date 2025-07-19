using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;
using BookManager.Data.Models;

public class BookManagerDbContext : IdentityDbContext
{
    public BookManagerDbContext(DbContextOptions<BookManagerDbContext> options)
        : base(options) { }

    public virtual DbSet<Book> Books { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.ApplyConfiguration(new UserRecipeConfiguration());
    }
}