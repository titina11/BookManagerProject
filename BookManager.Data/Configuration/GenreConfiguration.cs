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
            new Genre { Id = Guid.Parse("e6a6a80b-9eb6-4ce3-92b5-00b5cf9a53db"), Name = "Фентъзи" },
            new Genre { Id = Guid.Parse("ac233f36-92df-4d0f-9bfc-c2928eb38f88"), Name = "Любовни романи" },
            new Genre { Id = Guid.Parse("87a2bb5d-7884-49ea-93a3-2ea7fbe630b6"), Name = "Научна фантастика" }
        );
    }
}