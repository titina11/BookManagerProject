using BookManager.Data;
using BookManager.Data.Models;
using BookManager.ViewModels.Authors;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services.Core
{
    public class AuthorBookService : IAuthorBookService
    {
        private readonly BookManagerDbContext _context;

        public AuthorBookService(BookManagerDbContext context)
        {
            _context = context;
        }

        public async Task<AddBookToAuthorViewModel> GetAddBookModelAsync(Guid authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null) throw new ArgumentException("Invalid author ID.");

            var books = await _context.Books
                .Where(b => b.AuthorId != authorId)
                .Select(b => new BookDropdownViewModel
                {
                    Id = b.Id,
                    Title = b.Title
                }).ToListAsync();

            return new AddBookToAuthorViewModel
            {
                AuthorId = authorId,
                Books = books
            };
        }

        public async Task AddBookToAuthorAsync(AddBookToAuthorViewModel model)
        {
            var book = await _context.Books.FindAsync(model.SelectedBookId);
            if (book == null) throw new ArgumentException("Invalid book ID.");

            book.AuthorId = model.AuthorId;
            await _context.SaveChangesAsync();
        }
    }
}