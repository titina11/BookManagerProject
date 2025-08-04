using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services.Core;
using BookManager.ViewModels.Read;
using BookManager.ViewModels.UserBooks;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookManager.Tests.Services.Core
{
    public class ReadBookServiceTests
    {
        private BookManagerDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<BookManagerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new BookManagerDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUserBook()
        {
            var context = GetDbContext();
            var service = new ReadBookService(context);
            var userId = "user123";
            var bookId = Guid.NewGuid();

            context.Books.Add(new Book
            {
                Id = bookId,
                Title = "Тестова книга",
                Description = "Описание на книгата",    
                CreatedByUserId = userId                 
            });
            await context.SaveChangesAsync();

            var model = new ReadBookFormModel
            {
                BookId = bookId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(5),
                Rating = 5
            };

            await service.AddAsync(userId, model);

            Assert.Single(context.UserBooks);
            var userBook = context.UserBooks.First();
            Assert.Equal(userId, userBook.UserId);
            Assert.Equal(bookId, userBook.BookId);
            Assert.Equal(5, userBook.Rating);
        }

        [Fact]
        public async Task GetByUserAsync_ShouldReturnCorrectBooks()
        {
            var context = GetDbContext();
            var userId = "user456";
            var bookId = Guid.NewGuid();

            var book = new Book
            {
                Id = bookId,
                Title = "C# книга",
                Description = "Описание",            
                CreatedByUserId = userId              
            };

            var userBook = new UserBook
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                StartDate = DateTime.Today.AddDays(-10),
                EndDate = DateTime.Today,
                Rating = 4,
                Book = book
            };

            context.Books.Add(book);
            context.UserBooks.Add(userBook);
            await context.SaveChangesAsync();

            var service = new ReadBookService(context);

            var result = (await service.GetByUserAsync(userId)).ToList();

            Assert.Single(result);
            Assert.Equal(bookId, result[0].BookId);
            Assert.Equal("C# книга", result[0].BookTitle);
            Assert.Equal(4, result[0].Rating);
        }
    }
}
