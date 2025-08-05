using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services.Core;
using BookManager.ViewModels.Review;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookManager.Tests.Services
{
    public class ReviewServiceTests
    {
        private BookManagerDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<BookManagerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new BookManagerDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddReview()
        {
            var context = GetDbContext();
            var service = new ReviewService(context);
            var userId = "user123";
            var bookId = Guid.NewGuid();

            context.Books.Add(new Book
            {
                Id = bookId,
                Title = "Test Book",
                Description = "Description",
                CreatedByUserId = userId
            });
            await context.SaveChangesAsync();

            var model = new CreateReviewViewModel
            {
                BookId = bookId,
                Content = "Great book!",
                Rating = 5
            };

            await service.CreateAsync(model, userId);

            var review = await context.Reviews.FirstOrDefaultAsync();
            Assert.NotNull(review);
            Assert.Equal("Great book!", review.Content);
            Assert.Equal(5, review.Rating);
            Assert.Equal(userId, review.UserId);
        }

        [Fact]
        public async Task GetByBookIdAsync_ShouldReturnReviews()
        {
            var context = GetDbContext();
            var bookId = Guid.NewGuid();
            var userId = "user1";

            var book = new Book
            {
                Id = bookId,
                Title = "Book 1",
                Description = "Desc",
                CreatedByUserId = userId
            };

            var user = new ApplicationUser { Id = userId, Email = "user@example.com" };

            context.Books.Add(book);
            context.Users.Add(user);
            context.Reviews.Add(new Review
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                UserId = userId,
                Content = "Test review",
                Rating = 4,
                User = user
            });
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            var result = (await service.GetByBookIdAsync(bookId)).ToList();

            Assert.Single(result);
            Assert.Equal("Test review", result[0].Content);
            Assert.Equal("user@example.com", result[0].UserEmail);
        }


        [Fact]
        public async Task GetEditModelAsync_ShouldReturnModel_WhenReviewExists()
        {
            var context = GetDbContext();

            var review = new Review
            {
                Id = Guid.NewGuid(),
                Content = "Original review",
                Rating = 4,
                BookId = Guid.NewGuid(),
                UserId = "user1"
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            var result = await service.GetEditModelAsync(review.Id);

            Assert.NotNull(result);
            Assert.Equal(review.Id, result.Id);
            Assert.Equal(review.Content, result.Content);
            Assert.Equal(review.Rating, result.Rating);
            Assert.Equal(review.BookId, result.BookId);
            Assert.Equal(review.UserId, result.UserId);
        }

        [Fact]
        public async Task GetEditModelAsync_ShouldReturnNull_WhenReviewNotFound()
        {
            var context = GetDbContext();
            var service = new ReviewService(context);

            var result = await service.GetEditModelAsync(Guid.NewGuid()); 

            Assert.Null(result);
        }

        [Fact]
        public async Task EditReviewAsync_ShouldUpdateContent_WhenUserIsOwner()
        {
            var context = GetDbContext();
            var userId = "user123";
            var reviewId = Guid.NewGuid();

            var review = new Review
            {
                Id = reviewId,
                BookId = Guid.NewGuid(),
                UserId = userId,
                Content = "Old content",
                Rating = 3
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);
            var model = new EditReviewViewModel
            {
                Content = "Updated content",
                Rating = 5,
                BookId = review.BookId,
                UserId = userId
            };

            await service.EditReviewAsync(reviewId, model, userId, isAdmin: false);

            var updated = await context.Reviews.FindAsync(reviewId);
            Assert.Equal("Updated content", updated.Content);
            Assert.Equal(5, updated.Rating);
        }

        [Fact]
        public async Task EditReviewAsync_ShouldAllowEdit_WhenUserIsAdmin()
        {
            var context = GetDbContext();

            var review = new Review
            {
                Id = Guid.NewGuid(),
                Content = "Initial content",
                Rating = 3,
                BookId = Guid.NewGuid(),
                UserId = "realUser"
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            var model = new EditReviewViewModel
            {
                Content = "Updated by admin",
                Rating = 5,
                BookId = review.BookId,
                UserId = "anyone"
            };

            await service.EditReviewAsync(review.Id, model, currentUserId: "adminUser", isAdmin: true);

            var updated = await context.Reviews.FindAsync(review.Id);
            Assert.Equal("Updated by admin", updated.Content);
            Assert.Equal(5, updated.Rating);
        }

        [Fact]
        public async Task EditReviewAsync_ShouldDoNothing_WhenReviewNotFound()
        {
            var context = GetDbContext();

            var service = new ReviewService(context);

            var model = new EditReviewViewModel
            {
                Content = "Should not matter",
                Rating = 1,
                BookId = Guid.NewGuid(),
                UserId = "user"
            };

            await service.EditReviewAsync(Guid.NewGuid(), model, "user", false);

            Assert.Empty(context.Reviews);
        }

        [Fact]
        public async Task EditReviewAsync_ShouldThrow_WhenUserNotOwnerAndNotAdmin()
        {
            var context = GetDbContext();
            var reviewId = Guid.NewGuid();

            context.Reviews.Add(new Review
            {
                Id = reviewId,
                BookId = Guid.NewGuid(),
                UserId = "owner123",
                Content = "Text",
                Rating = 2
            });
            await context.SaveChangesAsync();

            var service = new ReviewService(context);
            var model = new EditReviewViewModel
            {
                Content = "Hacked!",
                Rating = 1,
                BookId = Guid.NewGuid(),
                UserId = "someone_else"
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await service.EditReviewAsync(reviewId, model, "someone_else", isAdmin: false));
        }


        [Fact]
        public async Task GetCreateModelAsync_ShouldReturnCorrectModel_WhenBookExists()
        {
            var context = GetDbContext();

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book",
                Description = "Test Desc",
                Author = new Author { Id = Guid.NewGuid(), Name = "Author", CreatedByUserId = "user" },
                Genre = new Genre { Id = Guid.NewGuid(), Name = "Genre" },
                Publisher = new Publisher { Id = Guid.NewGuid(), Name = "Publisher" },
                CreatedByUserId = "user"
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            var model = await service.GetCreateModelAsync(book.Id);

            Assert.Equal(book.Id, model.BookId);
            Assert.Equal(book.Title, model.BookTitle);
        }

        [Fact]
        public async Task GetCreateModelAsync_ShouldReturnDefaultTitle_WhenBookDoesNotExist()
        {
            var context = GetDbContext();
            var service = new ReviewService(context);

            var nonExistingId = Guid.NewGuid();

            var model = await service.GetCreateModelAsync(nonExistingId);

            Assert.Equal(nonExistingId, model.BookId);
            Assert.Equal("Неизвестна книга", model.BookTitle);
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldRemoveReview_WhenUserIsAdmin()
        {
           var context = GetDbContext();
            var reviewId = Guid.NewGuid();

            context.Reviews.Add(new Review
            {
                Id = reviewId,
                BookId = Guid.NewGuid(),
                UserId = "user",
                Content = "To delete",
                Rating = 1
            });
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            await service.DeleteReviewAsync(reviewId, "other_user", isAdmin: true);

            Assert.Empty(context.Reviews);
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldDeleteReview_WhenUserIsOwner()
        {
            var context = GetDbContext();

            var review = new Review
            {
                Id = Guid.NewGuid(),
                Content = "To be deleted by owner",
                Rating = 4,
                BookId = Guid.NewGuid(),
                UserId = "owner123"
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            await service.DeleteReviewAsync(review.Id, currentUserId: "owner123", isAdmin: false);

            var deleted = await context.Reviews.FindAsync(review.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldThrow_WhenUserIsNotOwnerOrAdmin()
        {
            var context = GetDbContext();

            var review = new Review
            {
                Id = Guid.NewGuid(),
                Content = "To be deleted",
                Rating = 3,
                BookId = Guid.NewGuid(),
                UserId = "realOwner"
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.DeleteReviewAsync(review.Id, "intruderUser", isAdmin: false));
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldDoNothing_WhenReviewNotFound()
        {
            var context = GetDbContext();
            var service = new ReviewService(context);

            await service.DeleteReviewAsync(Guid.NewGuid(), "anyUser", isAdmin: false);

            Assert.Empty(context.Reviews);
        }

        [Fact]
        public async Task GetReviewByIdAsync_ShouldReturnCorrectDetails()
        {
            var context = GetDbContext();
            var reviewId = Guid.NewGuid();
            var userId = "user123";

            var user = new ApplicationUser { Id = userId, Email = "user@example.com" };

            context.Users.Add(user);
            context.Reviews.Add(new Review
            {
                Id = reviewId,
                BookId = Guid.NewGuid(),
                UserId = userId,
                Content = "Detailed review",
                Rating = 4,
                User = user
            });
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            var result = await service.GetReviewByIdAsync(reviewId);

            Assert.NotNull(result);
            Assert.Equal("Detailed review", result.Content);
            Assert.Equal("user@example.com", result.UserEmail);
        }

        [Fact]
        public async Task GetReviewsForBookAsync_ShouldReturnAllReviews_ForBook()
        {
            var context = GetDbContext();
            var bookId = Guid.NewGuid();
            var user = new ApplicationUser { Id = "userX", Email = "x@example.com" };

            context.Users.Add(user);
            context.Reviews.AddRange(
                new Review
                {
                    Id = Guid.NewGuid(),
                    BookId = bookId,
                    UserId = user.Id,
                    Content = "First review",
                    Rating = 4,
                    User = user
                },
                new Review
                {
                    Id = Guid.NewGuid(),
                    BookId = bookId,
                    UserId = user.Id,
                    Content = "Second review",
                    Rating = 5,
                    User = user
                }
            );
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            var result = await service.GetReviewsForBookAsync(bookId);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Content == "First review");
            Assert.Contains(result, r => r.Content == "Second review");
        }
    }
}
