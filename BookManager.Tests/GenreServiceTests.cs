using BookManager.Data.Models;
using BookManager.Services.Core;
using BookManager.ViewModels.Genre;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BookManager.Tests;

public class GenreServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGenres()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "BookManagerTestDb")
            .Options;

        using (var context = new BookManagerDbContext(options))
        {
            context.Genres.Add(new Genre { Id = Guid.NewGuid(), Name = "Фентъзи" });
            context.Genres.Add(new Genre { Id = Guid.NewGuid(), Name = "Исторически" });
            await context.SaveChangesAsync();
        }

        using (var context = new BookManagerDbContext(options))
        {
            var service = new GenreService(context);

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, g => g.Name == "Фентъзи");
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectGenreViewModel()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "GetGenreByIdDb")
            .Options;

        var genreId = Guid.NewGuid();
        var genreName = "Класика";

        using (var context = new BookManagerDbContext(options))
        {
            context.Genres.Add(new Genre
            {
                Id = genreId,
                Name = genreName
            });
            await context.SaveChangesAsync();
        }

        using (var context = new BookManagerDbContext(options))
        {
            var service = new GenreService(context);
            var result = await service.GetByIdAsync(genreId);

            Assert.NotNull(result);
            Assert.Equal(genreId, result!.Id);
            Assert.Equal(genreName, result.Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_IfNotFound()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "GetGenreById_NullDb")
            .Options;

        using (var context = new BookManagerDbContext(options))
        {
            var service = new GenreService(context);

            var result = await service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldAddGenreToDatabase()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "CreateGenreDb")
            .Options;

        var newGenre = new GenreViewModel
        {
            Name = "Научна фантастика"
        };

        using (var context = new BookManagerDbContext(options))
        {
            var service = new GenreService(context);
            await service.CreateAsync(newGenre);
        }

        using (var context = new BookManagerDbContext(options))
        {
            var genres = await context.Genres.ToListAsync();
            Assert.Single(genres);
            Assert.Equal("Научна фантастика", genres[0].Name);
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateGenreName()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateGenreDb")
            .Options;

        var genreId = Guid.NewGuid();

        using (var context = new BookManagerDbContext(options))
        {
            context.Genres.Add(new Genre { Id = genreId, Name = "Старо име" });
            await context.SaveChangesAsync();
        }

        using (var context = new BookManagerDbContext(options))
        {
            var service = new GenreService(context);
            var updatedModel = new GenreViewModel
            {
                Id = genreId,
                Name = "Ново име"
            };

            await service.UpdateAsync(updatedModel);
        }

        using (var context = new BookManagerDbContext(options))
        {
            var genre = await context.Genres.FindAsync(genreId);
            Assert.NotNull(genre);
            Assert.Equal("Ново име", genre!.Name);
        }
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveGenre()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "DeleteGenreDb")
            .Options;

        var genreId = Guid.NewGuid();

        using (var context = new BookManagerDbContext(options))
        {
            context.Genres.Add(new Genre { Id = genreId, Name = "За изтриване" });
            await context.SaveChangesAsync();
        }

        using (var context = new BookManagerDbContext(options))
        {
            var service = new GenreService(context);
            await service.DeleteAsync(genreId);
        }

        using (var context = new BookManagerDbContext(options))
        {
            var genre = await context.Genres.FindAsync(genreId);
            Assert.Null(genre);
        }
    }
}