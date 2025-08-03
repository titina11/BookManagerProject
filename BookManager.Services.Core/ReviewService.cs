using BookManager.Data.Models;
using BookManager.ViewModels;
using BookManager.ViewModels.Review;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

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
            .Select(r => new ReviewViewModel
            {
                Content = r.Content,
                Rating = r.Rating,
                UserEmail = r.User.Email
            })
            .ToListAsync();
    }

    public async Task AddReviewAsync(CreateReviewViewModel model, string userId)
    {
        var review = new Review
        { Id = Guid.NewGuid(),
            Content = model.Content,
            Rating = model.Rating,
            BookId = model.BookId,
            UserId = userId
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
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
                UserEmail = r.User.Email
            })
            .ToListAsync();
    }
}
