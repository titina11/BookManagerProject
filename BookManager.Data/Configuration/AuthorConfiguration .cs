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

        const string defaultUserId = "12345678-abcd-1234-abcd-1234567890ab";

        builder.HasData(
            new Author
            {
                Id = Guid.Parse("9e340fd5-7f9e-43dc-96f0-07a3b9a1b12a"),
                Name = "Кариса Броудбент",
                CreatedByUserId = defaultUserId
            },
            new Author
            {
                Id = Guid.Parse("264a2a30-ec23-4aef-b1cb-8c7a4c9f7fa4"),
                Name = "Сара Дж. Маас",
                CreatedByUserId = defaultUserId
            },
            new Author
            {
                Id = Guid.Parse("fdddc2cb-718a-4cf3-9a8c-490c61cd31ae"),
                Name = "Джордж Р.Р.Мартин",
                CreatedByUserId = defaultUserId
            }
        );
    }
}