using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookManager.Data.Models;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Description)
            .HasMaxLength(2000);

        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Genre)
            .WithMany(g => g.Books)
            .HasForeignKey(b => b.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(b => b.ImageUrl)
            .HasMaxLength(500);

        builder.HasOne(b => b.CreatedByUser)
            .WithMany()
            .HasForeignKey(b => b.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        const string defaultUserId = "12345678-abcd-1234-abcd-1234567890ab";

        builder.HasData(
            new Book
            {
                Id = Guid.Parse("7a1a29d6-70c4-4c02-9469-3b92d3d9b7ee"),
                Title = "Змията и крилете на нощта",
                AuthorId = Guid.Parse("9e340fd5-7f9e-43dc-96f0-07a3b9a1b12a"),
                GenreId = Guid.Parse("e6a6a80b-9eb6-4ce3-92b5-00b5cf9a53db"),
                PublisherId = Guid.Parse("1f76d1f6-5c97-42b1-a5c7-e685b1541c1b"),
                Description = "…",
                ImageUrl = "https://knigoman.bg/books/2303525764_1552613569786.png",
                CreatedByUserId = defaultUserId
            },
            new Book
            {
                Id = Guid.Parse("89c68c41-e015-4bc2-8c03-4fbd7a0f2678"),
                Title = "Стъкленият трон",
                AuthorId = Guid.Parse("264a2a30-ec23-4aef-b1cb-8c7a4c9f7fa4"),
                GenreId = Guid.Parse("e6a6a80b-9eb6-4ce3-92b5-00b5cf9a53db"),
                PublisherId = Guid.Parse("2a9cd570-96b6-4f52-b56d-137e2c5d5eaf"),
                Description = "…",
                ImageUrl = "https://cdn.ozone.bg/media/catalog/product/s/t/stakleniyat_tron_…",
                CreatedByUserId = defaultUserId
            }
        );
    }
}