using BookManager.Data.Models;
using BookManager.ViewModels.Read;
using BookManager.ViewModels.UserBooks;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services.Core;

public class ReadBookService : IReadBookService
{
    private readonly BookManagerDbContext _context;

    public ReadBookService(BookManagerDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<ReadBookViewModel>> GetByUserAsync(string userId)
    {
        return await _context.UserBooks
            .Where(r => r.UserId == userId)
            .Include(r => r.Book)
            .Select(r => new ReadBookViewModel
            {
                Id = r.Id,
                BookId = r.BookId,
                BookTitle = r.Book.Title,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Rating = r.Rating
            }).ToListAsync();
    }



    public async Task AddAsync(string userId, ReadBookFormModel model)
    {
        var entity = new UserBook()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = model.BookId,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Rating = model.Rating
        };

        _context.UserBooks.Add(entity);
        await _context.SaveChangesAsync();
    }


}