using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services.Core;
using BookManager.ViewModels.Book;
using BookManager.ViewModels.Books;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookManager.Tests.Services.Core
{
    public class BookServiceTests
    {
        private BookManagerDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<BookManagerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new BookManagerDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBooks()
        {
            var context = GetDbContext();

            var author = new Author { Id = Guid.NewGuid(), Name = "Author A", CreatedByUserId = "admin" };
            var genre = new Genre { Id = Guid.NewGuid(), Name = "Genre A" };
            var publisher = new Publisher { Id = Guid.NewGuid(), Name = "Publisher A" };

            context.Authors.Add(author);
            context.Genres.Add(genre);
            context.Publishers.Add(publisher);

            context.Books.Add(new Book
            {
                Id = Guid.NewGuid(),
                Title = "Book 1",
                Description = "Description 1",
                AuthorId = author.Id,
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                CreatedByUserId = "admin",
                ImageUrl = "image1.jpg"
            });

            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetAllAsync();

            Assert.Single(result);
            Assert.Equal("Book 1", result.First().Title);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnBook_WhenExists()
        {
            var context = GetDbContext();

            var author = new Author { Id = Guid.NewGuid(), Name = "Author A", CreatedByUserId = "admin" };
            var genre = new Genre { Id = Guid.NewGuid(), Name = "Genre A" };
            var publisher = new Publisher { Id = Guid.NewGuid(), Name = "Publisher A" };

            context.Authors.Add(author);
            context.Genres.Add(genre);
            context.Publishers.Add(publisher);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Book 2",
                Description = "Desc",
                AuthorId = author.Id,
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                CreatedByUserId = "admin",
                ImageUrl = "img.jpg"
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetByIdAsync(book.Id);

            Assert.NotNull(result);
            Assert.Equal("Book 2", result.Title);
            Assert.Equal("Author A", result.Author);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateBookWithNewAuthorGenrePublisher()
        {
            var context = GetDbContext();
            var service = new BookService(context);
            var userId = "user1";

            var model = new CreateBookViewModel
            {
                Title = "New Book",
                Description = "Book Desc",
                ImageUrl = "image.jpg",
                NewAuthorName = "New Author",
                NewGenreName = "New Genre",
                NewPublisherName = "New Publisher"
            };

            await service.CreateAsync(model, userId);

            var book = await context.Books.Include(b => b.Author).Include(b => b.Genre).Include(b => b.Publisher).FirstOrDefaultAsync();
            Assert.NotNull(book);
            Assert.Equal("New Book", book.Title);
            Assert.Equal("New Author", book.Author.Name);
            Assert.Equal("New Genre", book.Genre.Name);
            Assert.Equal("New Publisher", book.Publisher.Name);
            Assert.Equal(userId, book.CreatedByUserId);
        }

        [Fact]
        public async Task GetEditModelAsync_ShouldReturnEditModel_WhenBookExists()
        {
            var context = GetDbContext();
            var author = new Author { Id = Guid.NewGuid(), Name = "Author", CreatedByUserId = "admin" };
            var genre = new Genre { Id = Guid.NewGuid(), Name = "Genre" };
            var publisher = new Publisher { Id = Guid.NewGuid(), Name = "Publisher" };

            context.Authors.Add(author);
            context.Genres.Add(genre);
            context.Publishers.Add(publisher);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Editable Book",
                Description = "Desc",
                AuthorId = author.Id,
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                ImageUrl = "img.jpg",
                CreatedByUserId = "admin"
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetEditModelAsync(book.Id);

            Assert.NotNull(result);
            Assert.Equal("Editable Book", result.Title);
            Assert.Equal(author.Id, result.AuthorId);
            Assert.Equal(genre.Id, result.GenreId);
            Assert.Equal(publisher.Id, result.PublisherId);
            Assert.NotEmpty(result.Authors);
            Assert.NotEmpty(result.Genres);
            Assert.NotEmpty(result.Publishers);
        }

        [Fact]
        public async Task EditAsync_ShouldUpdateBook_WhenUserIsOwner()
        {
            var context = GetDbContext();
            var author = new Author { Id = Guid.NewGuid(), Name = "Old Author", CreatedByUserId = "admin" };
            var genre = new Genre { Id = Guid.NewGuid(), Name = "Old Genre" };
            var publisher = new Publisher { Id = Guid.NewGuid(), Name = "Old Publisher" };

            var newAuthor = new Author { Id = Guid.NewGuid(), Name = "New Author", CreatedByUserId = "admin" };
            var newGenre = new Genre { Id = Guid.NewGuid(), Name = "New Genre" };
            var newPublisher = new Publisher { Id = Guid.NewGuid(), Name = "New Publisher" };

            context.Authors.AddRange(author, newAuthor);
            context.Genres.AddRange(genre, newGenre);
            context.Publishers.AddRange(publisher, newPublisher);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Old Title",
                Description = "Old Desc",
                AuthorId = author.Id,
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                ImageUrl = "old.jpg",
                CreatedByUserId = "owner1"
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var model = new EditBookViewModel
            {
                Title = "Updated Title",
                Description = "Updated Desc",
                ImageUrl = "new.jpg",
                AuthorId = newAuthor.Id,
                GenreId = newGenre.Id,
                PublisherId = newPublisher.Id
            };

            await service.EditAsync(book.Id, model, currentUserId: "owner1", isAdmin: false);

            var updated = await context.Books.FindAsync(book.Id);
            Assert.Equal("Updated Title", updated.Title);
            Assert.Equal("Updated Desc", updated.Description);
            Assert.Equal("new.jpg", updated.ImageUrl);
            Assert.Equal(newAuthor.Id, updated.AuthorId);
            Assert.Equal(newGenre.Id, updated.GenreId);
            Assert.Equal(newPublisher.Id, updated.PublisherId);
        }

        [Fact]
        public async Task EditAsync_ShouldNotUpdate_WhenUserIsNotOwnerAndNotAdmin()
        {
            var context = GetDbContext();

            var author = new Author { Id = Guid.NewGuid(), Name = "Author", CreatedByUserId = "admin" };
            var genre = new Genre { Id = Guid.NewGuid(), Name = "Genre" };
            var publisher = new Publisher { Id = Guid.NewGuid(), Name = "Publisher" };

            context.Authors.Add(author);
            context.Genres.Add(genre);
            context.Publishers.Add(publisher);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Initial Title",
                Description = "Initial Desc",
                AuthorId = author.Id,
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                ImageUrl = "initial.jpg",
                CreatedByUserId = "realOwner"
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var model = new EditBookViewModel
            {
                Title = "Should Not Update",
                Description = "Should Not Update",
                ImageUrl = "fail.jpg",
                AuthorId = author.Id,
                GenreId = genre.Id,
                PublisherId = publisher.Id
            };

            await service.EditAsync(book.Id, model, currentUserId: "intruder", isAdmin: false);

            var unchanged = await context.Books.FindAsync(book.Id);
            Assert.Equal("Initial Title", unchanged.Title);
            Assert.Equal("Initial Desc", unchanged.Description);
            Assert.Equal("initial.jpg", unchanged.ImageUrl);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldReturnFilteredBooks()
        {
            var context = GetDbContext();
            var author1 = new Author { Id = Guid.NewGuid(), Name = "John Doe", CreatedByUserId = "admin" };
            var genre1 = new Genre { Id = Guid.NewGuid(), Name = "Sci-Fi" };
            var publisher1 = new Publisher { Id = Guid.NewGuid(), Name = "MegaPub" };

            context.Authors.Add(author1);
            context.Genres.Add(genre1);
            context.Publishers.Add(publisher1);

            context.Books.AddRange(
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Space Odyssey",
                    Description = "Sci-fi space book",
                    AuthorId = author1.Id,
                    GenreId = genre1.Id,
                    PublisherId = publisher1.Id,
                    CreatedByUserId = "admin"
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Romantic Tale",
                    Description = "Love story",
                    AuthorId = author1.Id,
                    GenreId = genre1.Id,
                    PublisherId = publisher1.Id,
                    CreatedByUserId = "admin"
                }
            );
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetFilteredAsync("space", author1.Id, genre1.Id, publisher1.Id);

            Assert.Single(result.Books);
            Assert.Equal("Space Odyssey", result.Books[0].Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveBook_WhenExists()
        {
            var context = GetDbContext();

            var author = new Author { Id = Guid.NewGuid(), Name = "A", CreatedByUserId = "admin" };
            var genre = new Genre { Id = Guid.NewGuid(), Name = "G" };
            var publisher = new Publisher { Id = Guid.NewGuid(), Name = "P" };

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "To Delete",
                Description = "Desc",
                AuthorId = author.Id,
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                CreatedByUserId = "admin"
            };

            context.Authors.Add(author);
            context.Genres.Add(genre);
            context.Publishers.Add(publisher);
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var service = new BookService(context);

            await service.DeleteAsync(book.Id);

            var deleted = await context.Books.FindAsync(book.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetCreateModelAsync_ShouldReturnDropdownLists()
        {
            var context = GetDbContext();

            context.Authors.Add(new Author { Id = Guid.NewGuid(), Name = "Author X", CreatedByUserId = "admin" });
            context.Genres.Add(new Genre { Id = Guid.NewGuid(), Name = "Genre X" });
            context.Publishers.Add(new Publisher { Id = Guid.NewGuid(), Name = "Publisher X" });

            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetCreateModelAsync();

            Assert.Single(result.Authors);
            Assert.Single(result.Genres);
            Assert.Single(result.Publishers);
        }

        [Fact]
        public async Task GetDetailsByIdAsync_ShouldReturnFullDetails_WhenBookExists()
        {
            var context = GetDbContext();

            var author = new Author { Id = Guid.NewGuid(), Name = "A", CreatedByUserId = "admin" };
            var genre = new Genre { Id = Guid.NewGuid(), Name = "G" };
            var publisher = new Publisher { Id = Guid.NewGuid(), Name = "P" };
            var bookId = Guid.NewGuid();

            var book = new Book
            {
                Id = bookId,
                Title = "Book D",
                Description = "Details",
                AuthorId = author.Id,
                GenreId = genre.Id,
                PublisherId = publisher.Id,
                CreatedByUserId = "admin",
                ImageUrl = "img.jpg"
            };

            context.Authors.Add(author);
            context.Genres.Add(genre);
            context.Publishers.Add(publisher);
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetDetailsByIdAsync(bookId);

            Assert.NotNull(result);
            Assert.Equal("Book D", result.Title);
            Assert.Equal("A", result.Author);
            Assert.Equal("G", result.Genre);
            Assert.Equal("P", result.Publisher);
        }

        [Fact]
        public async Task GetLatestAsync_ShouldReturnLatestBooks()
        {
            var context = GetDbContext();

            var author = new Author { Id = Guid.NewGuid(), Name = "Author", CreatedByUserId = "admin" };
            var genre = new Genre { Id = Guid.NewGuid(), Name = "Genre" };
            var publisher = new Publisher { Id = Guid.NewGuid(), Name = "Publisher" };

            context.Authors.Add(author);
            context.Genres.Add(genre);
            context.Publishers.Add(publisher);

            var oldBookId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var newBookId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

            context.Books.AddRange(
                new Book
                {
                    Id = oldBookId,
                    Title = "Old Book",
                    Description = "Desc",
                    AuthorId = author.Id,
                    GenreId = genre.Id,
                    PublisherId = publisher.Id,
                    CreatedByUserId = "admin"
                },
                new Book
                {
                    Id = newBookId,
                    Title = "New Book",
                    Description = "Desc",
                    AuthorId = author.Id,
                    GenreId = genre.Id,
                    PublisherId = publisher.Id,
                    CreatedByUserId = "admin"
                }
            );
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetLatestAsync(1);

            Assert.Single(result);
            Assert.Equal("New Book", result.First().Title);
        }

        [Fact]
        public async Task GetAuthorsAsync_ShouldReturnAllAuthors()
        {
            var context = GetDbContext();
            context.Authors.Add(new Author { Id = Guid.NewGuid(), Name = "Author A", CreatedByUserId = "admin" });
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetAuthorsAsync();

            Assert.Single(result);
            Assert.Equal("Author A", result.First().Name);
        }

        [Fact]
        public async Task GetGenresAsync_ShouldReturnAllGenres()
        {
            var context = GetDbContext();
            context.Genres.Add(new Genre { Id = Guid.NewGuid(), Name = "Genre B" });
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetGenresAsync();

            Assert.Single(result);
            Assert.Equal("Genre B", result.First().Name);
        }

        [Fact]
        public async Task GetPublishersAsync_ShouldReturnAllPublishers()
        {
            var context = GetDbContext();
            context.Publishers.Add(new Publisher { Id = Guid.NewGuid(), Name = "Publisher C" });
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var result = await service.GetPublishersAsync();

            Assert.Single(result);
            Assert.Equal("Publisher C", result.First().Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateBook_WhenBookExists()
        {
            var context = GetDbContext();

            var author1 = new Author { Id = Guid.NewGuid(), Name = "A1", CreatedByUserId = "admin" };
            var genre1 = new Genre { Id = Guid.NewGuid(), Name = "G1" };
            var publisher1 = new Publisher { Id = Guid.NewGuid(), Name = "P1" };

            var author2 = new Author { Id = Guid.NewGuid(), Name = "A2", CreatedByUserId = "admin" };
            var genre2 = new Genre { Id = Guid.NewGuid(), Name = "G2" };
            var publisher2 = new Publisher { Id = Guid.NewGuid(), Name = "P2" };

            context.Authors.AddRange(author1, author2);
            context.Genres.AddRange(genre1, genre2);
            context.Publishers.AddRange(publisher1, publisher2);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Old Title",
                Description = "Old Desc",
                ImageUrl = "old.jpg",
                AuthorId = author1.Id,
                GenreId = genre1.Id,
                PublisherId = publisher1.Id,
                CreatedByUserId = "userX"
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var service = new BookService(context);

            var model = new BookFormModel
            {
                Id = book.Id,
                Title = "Updated Title",
                Description = "Updated Desc",
                ImageUrl = "updated.jpg",
                AuthorId = author2.Id,
                GenreId = genre2.Id,
                PublisherId = publisher2.Id
            };

            await service.UpdateAsync(model);

            var updated = await context.Books.FindAsync(book.Id);
            Assert.Equal("Updated Title", updated.Title);
            Assert.Equal("Updated Desc", updated.Description);
            Assert.Equal("updated.jpg", updated.ImageUrl);
            Assert.Equal(author2.Id, updated.AuthorId);
            Assert.Equal(genre2.Id, updated.GenreId);
            Assert.Equal(publisher2.Id, updated.PublisherId);
        }



    }
}
