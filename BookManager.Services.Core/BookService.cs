using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Book;
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
                .Include(b => b.Publisher)
                .Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Author = b.Author.Name,
                    Genre = b.Genre.Name,
                    Publisher = b.Publisher.Name,
                    ImageUrl = b.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<BookViewModel?> GetByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return null;

            return new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author.Name,
                Genre = book.Genre.Name,
                Publisher = book.Publisher.Name,
                ImageUrl = book.ImageUrl
            };
        }

        public async Task<CreateBookViewModel> GetCreateModelAsync()
        {
            return new CreateBookViewModel
            {
                Authors = await _context.Authors
                    .Select(a => new AuthorDropdownViewModel { Id = a.Id, Name = a.Name })
                    .ToListAsync(),
                Genres = await _context.Genres
                    .Select(g => new GenreDropdownViewModel { Id = g.Id, Name = g.Name })
                    .ToListAsync(),
                Publishers = await _context.Publishers
                    .Select(p => new PublisherDropdownViewModel { Id = p.Id, Name = p.Name })
                    .ToListAsync()
            };
        }

        public async Task<EditBookViewModel?> GetEditModelAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            return new EditBookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = book.AuthorId,
                GenreId = book.GenreId,
                PublisherId = book.PublisherId,
                ImageUrl = book.ImageUrl,
                Authors = await _context.Authors
                    .Select(a => new AuthorDropdownViewModel { Id = a.Id, Name = a.Name })
                    .ToListAsync(),
                Genres = await _context.Genres
                    .Select(g => new GenreDropdownViewModel { Id = g.Id, Name = g.Name })
                    .ToListAsync(),
                Publishers = await _context.Publishers
                    .Select(p => new PublisherDropdownViewModel { Id = p.Id, Name = p.Name })
                    .ToListAsync()
            };
        }

        public async Task CreateAsync(CreateBookViewModel model)
        {
            var book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                AuthorId = model.AuthorId,
                GenreId = model.GenreId,
                PublisherId = model.PublisherId,
                ImageUrl = model.ImageUrl
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(int id, EditBookViewModel model)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return;

            book.Title = model.Title;
            book.Description = model.Description;
            book.AuthorId = model.AuthorId;
            book.GenreId = model.GenreId;
            book.PublisherId = model.PublisherId;
            book.ImageUrl = model.ImageUrl;

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
