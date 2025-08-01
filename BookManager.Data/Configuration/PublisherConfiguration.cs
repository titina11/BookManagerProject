using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookManager.Data.Models;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.HasData(
            new Publisher
            {
                Id = Guid.Parse("1f76d1f6-5c97-42b1-a5c7-e685b1541c1b"),
                Name = "Студио Артлайн",
                Description = "Българско издателство за фентъзи и фантастика."
            },
            new Publisher
            {
                Id = Guid.Parse("2a9cd570-96b6-4f52-b56d-137e2c5d5eaf"),
                Name = "Егмонт",
                Description = "Популярно издателство за младежка литература и фентъзи."
            },
            new Publisher
            {
                Id = Guid.Parse("3ff6d54c-b01a-4cc5-b289-15d20b92df7d"),
                Name = "Бард",
                Description = "Издателство за световни бестселъри и фантастика."
            }
        );
    }
}