using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookManager.Data.Models;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(
            new Author
            {
                Id = 1,
                Name = "Кариса Броудбент"
            },
            new Author
            {
                Id = 2,
                Name = "Сара Дж. Маас"
            },
            new Author
            {
                Id = 3,
                Name = "Джордж Р.Р.Мартин"
            }
        );
    }
}