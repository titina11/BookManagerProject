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
            new Publisher { Id = 1, Name = "Студио Артлайн" },
            new Publisher { Id = 2, Name = "Егмонт" },
            new Publisher { Id = 3, Name = "Бард" }
        );
    }
}