using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using BookManager.Web.Areas.Identity.Data;

namespace BookManager.Data
{
    public class BookManagerDbContextFactory : IDesignTimeDbContextFactory<BookManagerDbContext>
    {
        public BookManagerDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BookManager.Web"))
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<BookManagerDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new BookManagerDbContext(builder.Options);
        }
    }
}