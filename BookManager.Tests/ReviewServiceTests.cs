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

namespace BookManager.Tests.Services.Core
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
        public async Task GetEditModelAsync_ShouldReturnCorrectModel_WhenReviewExists()
        {
            var context = GetDbContext();
            var reviewId = Guid.NewGuid();
            var review = new Review
            {
                Id = reviewId,
                BookId = Guid.NewGuid(),
                UserId = "user1",
                Content = "Original",
                Rating = 3
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            var result = await service.GetEditModelAsync(reviewId);

            Assert.NotNull(result);
            Assert.Equal("Original", result.Content);
            Assert.Equal(3, result.Rating);
            Assert.Equal("user1", result.UserId);
            Assert.Equal(reviewId, result.Id);
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
