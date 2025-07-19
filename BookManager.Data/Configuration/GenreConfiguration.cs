using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookManager.Data.Models;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasData(
            new Genre { Id = 1, Name = "Фентъзи" },
            new Genre { Id = 2, Name = "Любовни романи" },
            new Genre { Id = 3, Name = "Научна фантастика" }
        );
    }
}