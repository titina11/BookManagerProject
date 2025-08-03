using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookManager.Data.Models;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasData(
            new ApplicationUser
            {
                Id = "12345678-abcd-1234-abcd-1234567890ab",
                UserName = "seeduser@example.com",
                NormalizedUserName = "SEEDUSER@EXAMPLE.COM",
                Email = "seeduser@example.com",
                NormalizedEmail = "SEEDUSER@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEFpUyPwhlTG4HOSov1cF6OWg4+jE7ZuOw2TCTmUyU/OSGfPoTOE48qMJ/VoSxSPmbw==", 
                SecurityStamp = "a638f80f-c0d2-43e1-9ff7-aeca5aca92da",
                ConcurrencyStamp = "94950648-b2ed-4705-9f53-9215993a1d23"
            }
        );
    }
}