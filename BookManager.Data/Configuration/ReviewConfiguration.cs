using BookManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookManager.Data.Configuration;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{

    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(r => r.Rating)
            .IsRequired();
    }
}