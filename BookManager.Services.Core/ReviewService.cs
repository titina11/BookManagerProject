using BookManager.Data.Models;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Review;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services.Core
{
    public class ReviewService : IReviewService
    {
        private readonly BookManagerDbContext _context;

        public ReviewService(BookManagerDbContext context)
        {
            _context = context;
        }

        public async Task<CreateReviewViewModel> GetCreateModelAsync(Guid bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            return new CreateReviewViewModel
            {
                BookId = bookId,
                BookTitle = book?.Title ?? "Неизвестна книга"
            };
        }

        public async Task CreateAsync(CreateReviewViewModel model, string userId)
        {
            var review = new Review
            {
                Id = Guid.NewGuid(),
                BookId = model.BookId,
                UserId = userId,
                Content = model.Content,
                Rating = model.Rating
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReviewViewModel>> GetByBookIdAsync(Guid bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.User)
                .OrderByDescending(r => r.Id)
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    Content = r.Content,
                    Rating = r.Rating,
                    UserEmail = r.User.Email,
                    BookId = r.BookId
                })
                .ToListAsync();
        }

        public async Task<EditReviewViewModel?> GetEditModelAsync(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return null;

            return new EditReviewViewModel
            {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                BookId = review.BookId,
                UserId = review.UserId
            };
        }

        public async Task EditReviewAsync(Guid id, EditReviewViewModel model, string currentUserId, bool isAdmin)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return;

            if (review.UserId != currentUserId && !isAdmin)
                throw new UnauthorizedAccessException();

            review.Content = model.Content;
            review.Rating = model.Rating;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(Guid id, string currentUserId, bool isAdmin)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return;

            if (review.UserId != currentUserId && !isAdmin)
                throw new UnauthorizedAccessException();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<ReviewDetailsViewModel?> GetReviewByIdAsync(Guid id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.Id == id)
                .Select(r => new ReviewDetailsViewModel
                {
                    Id = r.Id,
                    Content = r.Content,
                    Rating = r.Rating,
                    UserEmail = r.User.Email,
                    BookId = r.BookId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<ReviewViewModel>> GetReviewsForBookAsync(Guid bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.User)
                .OrderByDescending(r => r.Id)
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    Content = r.Content,
                    Rating = r.Rating,
                    UserEmail = r.User.Email,
                    BookId = r.BookId
                })
                .ToListAsync();
        }
    }
}
