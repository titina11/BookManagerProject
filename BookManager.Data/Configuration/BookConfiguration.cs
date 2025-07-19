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

        builder.HasData(
            new Book
            {
                Id = 1,
                Title = "Змията и крилете на нощта",
                AuthorId = 1,
                GenreId = 2,
                Description = "„Змията и крилете на нощта“, първата книга от популярната поредица „Короните на Наяксия“, любима на десетки хиляди читатели по света.",
                PublisherId = 1,
                ImageUrl = "https://knigoman.bg/books/2303525764_1552613569786.png"
            },
            new Book
            {
                Id = 2,
                Title = "Стъкленият трон",
                AuthorId = 2,
                GenreId = 1,
                Description = "Селена Сардотиен е измъкната от затвора на Ендовер и единственият начин да спечели свободата си е да се пребори с най-жестоките мъже за титлата - кралски убиец. Но под красивата външност на Селена се крие боец с убийствени инстинкти. А свободата си струва всяка пролята капка кръв - и собствената и чуждата.",
                PublisherId = 2,
                ImageUrl = "https://cdn.ozone.bg/media/catalog/product/s/t/stakleniyat_tron_stakleniyat_tron_1_novo_izdanie_1713431567_0.jpg"
            },
            new Book
            {
                Id = 3,
                Title = "Игра на тронове (Песен за огън и лед 1)",
                AuthorId = 3,
                GenreId = 1,
                Description = "Шеметен бяг от скована в жесток студ страна към земи на вечно лято и охолно безгрижие. Сказание за владетели и владетелки, воини и чародеи, наемни убийци и незаконнородени претенденти за власт, появили се във времена на мрачни поличби.",
                PublisherId = 3,
                ImageUrl = "https://cdn.ozone.bg/media/catalog/product/i/g/igra-na-tronove-pesen-za-og-n-i-led-1.jpg"
            }
        );
    }
}