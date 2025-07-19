using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services.Core
{
    public class BookService : IBookService
    {
        private readonly BookManagerDbContext _context;

        public BookService(BookManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookViewModel>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author.Name,
                    Genre = b.Genre.Name,
                    Description = b.Description
                })
                .ToListAsync();
        }

        public async Task<BookViewModel?> GetByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return null;

            return new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author.Name,
                Genre = book.Genre.Name,
                Description = book.Description
            };
        }

        public async Task CreateAsync(BookViewModel model)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == model.Author);
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == model.Genre);

            if (author == null || genre == null)
            {
                throw new ArgumentException("Invalid Author or Genre.");
            }

            var book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                AuthorId = author.Id,
                GenreId = genre.Id
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(int id, BookViewModel model)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return;

            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == model.Author);
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == model.Genre);

            if (author == null || genre == null)
            {
                throw new ArgumentException("Invalid Author or Genre.");
            }

            book.Title = model.Title;
            book.Description = model.Description;
            book.AuthorId = author.Id;
            book.GenreId = genre.Id;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
