using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services.Core;
using BookManager.ViewModels.Author;
using BookManager.ViewModels.Authors;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class AuthorServiceTests
{
    private BookManagerDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BookManagerDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddAuthor()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "CreateAuthorTest")
            .Options;

        var model = new CreateAuthorViewModel
        {
            Name = "Тестов Автор"
        };

        using (var dbContext = new BookManagerDbContext(options))
        {
            var service = new AuthorService(dbContext);

            await service.CreateAsync(model, createdByUserId: "test-user");
        }

        using (var dbContext = new BookManagerDbContext(options))
        {
            var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == "Тестов Автор");

            Assert.NotNull(author);
            Assert.Equal("Тестов Автор", author.Name);
            Assert.Equal("test-user", author.CreatedByUserId);
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAuthors()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_GetAllAuthors")
            .Options;

        var userId = "test-user-id";

        using (var dbContext = new BookManagerDbContext(options))
        {
            dbContext.Authors.AddRange(
                new Author { Id = Guid.NewGuid(), Name = "Автор 1", CreatedByUserId = userId },
                new Author { Id = Guid.NewGuid(), Name = "Автор 2", CreatedByUserId = userId }
            );
            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = new BookManagerDbContext(options))
        {
            var service = new AuthorService(dbContext);

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.Name == "Автор 1");
            Assert.Contains(result, a => a.Name == "Автор 2");
        }
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAuthor()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_DeleteAuthor")
            .Options;

        var authorId = Guid.NewGuid();

        using (var dbContext = new BookManagerDbContext(options))
        {
            dbContext.Authors.Add(new Author
            {
                Id = authorId,
                Name = "Тест Автор",
                CreatedByUserId = "test-user-id" 
            });

            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = new BookManagerDbContext(options))
        {
            var service = new AuthorService(dbContext);

            await service.DeleteAsync(authorId);

            var author = await dbContext.Authors.FindAsync(authorId);
            Assert.Null(author); 
        }
    }

    [Fact]
    public async Task AddBookToAuthorAsync_Should_UpdateAuthorId()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateAuthorIdTest")
            .Options;

        var bookId = Guid.NewGuid();
        var oldAuthorId = Guid.NewGuid();
        var newAuthorId = Guid.NewGuid();

        using (var dbContext = new BookManagerDbContext(options))
        {
            dbContext.Authors.Add(new Author
            {
                Id = oldAuthorId,
                Name = "Стар Автор",
                CreatedByUserId = "test"
            });

            dbContext.Authors.Add(new Author
            {
                Id = newAuthorId,
                Name = "Нов Автор",
                CreatedByUserId = "test"
            });

            dbContext.Books.Add(new Book
            {
                Id = bookId,
                Title = "Книга",
                Description = "Описание",
                AuthorId = oldAuthorId,
                CreatedByUserId = "test"
            });

            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = new BookManagerDbContext(options))
        {
            var service = new AuthorService(dbContext);

            var model = new AddBookToAuthorViewModel
            {
                AuthorId = newAuthorId,
                SelectedBookId = bookId
            };

            await service.AddBookToAuthorAsync(model);
        }

        using (var dbContext = new BookManagerDbContext(options))
        {
            var updatedBook = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            Assert.NotNull(updatedBook);
            Assert.Equal(newAuthorId, updatedBook.AuthorId);
        }
    }

    [Fact]
    public async Task GetAddBookModelAsync_ShouldReturnBooksNotAssignedToAuthor()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetAddBookModel")
            .Options;

        var authorId = Guid.NewGuid();
        var otherAuthorId = Guid.NewGuid();

        using (var context = new BookManagerDbContext(options))
        {
            context.Books.AddRange(
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Book 1",
                    AuthorId = Guid.Empty,
                    Description = "desc",
                    CreatedByUserId = "test"
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Book 2",
                    AuthorId = authorId, 
                    Description = "desc",
                    CreatedByUserId = "test"
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Book 3",
                    AuthorId = otherAuthorId,
                    Description = "desc",
                    CreatedByUserId = "test"
                }
            );

            await context.SaveChangesAsync();
        }

        using (var context = new BookManagerDbContext(options))
        {
            var service = new AuthorService(context);

            var result = await service.GetAddBookModelAsync(authorId);

            Assert.Equal(authorId, result.AuthorId);
            Assert.Equal(2, result.Books.Count); 
            Assert.DoesNotContain(result.Books, b => b.Title == "Book 2");
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAuthorViewModel_WhenAuthorExists()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var authorId = Guid.NewGuid();

        using (var context = new BookManagerDbContext(options))
        {
            context.Authors.Add(new Author
            {
                Id = authorId,
                Name = "Тест Автор",
                CreatedByUserId = "user123"
            });

            await context.SaveChangesAsync();
        }

        using (var context = new BookManagerDbContext(options))
        {
            var service = new AuthorService(context);
            var result = await service.GetByIdAsync(authorId);

            Assert.NotNull(result);
            Assert.IsType<EditAuthorViewModel>(result);
            Assert.Equal(authorId, result!.Id);
            Assert.Equal("Тест Автор", result.Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenAuthorDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var context = new BookManagerDbContext(options))
        {
            var service = new AuthorService(context);

            var result = await service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAuthorName_WhenAuthorExists()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var authorId = Guid.NewGuid();

        using (var context = new BookManagerDbContext(options))
        {
            context.Authors.Add(new Author
            {
                Id = authorId,
                Name = "Старо име",
                CreatedByUserId = "user123"
            });

            await context.SaveChangesAsync();
        }

        using (var context = new BookManagerDbContext(options))
        {
            var service = new AuthorService(context);
            var model = new EditAuthorViewModel
            {
                Id = authorId,
                Name = "Ново име"
            };

            await service.UpdateAsync(model);
        }

        using (var context = new BookManagerDbContext(options))
        {
            var updatedAuthor = await context.Authors.FindAsync(authorId);
            Assert.NotNull(updatedAuthor);
            Assert.Equal("Ново име", updatedAuthor!.Name);
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldDoNothing_WhenAuthorDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var context = new BookManagerDbContext(options))
        {
            var service = new AuthorService(context);
            var model = new EditAuthorViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Несъществуващ"
            };

            await service.UpdateAsync(model);

            Assert.Empty(context.Authors);
        }
    }
}
