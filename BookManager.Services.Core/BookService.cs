using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Author;
using BookManager.ViewModels.Book;
using BookManager.ViewModels.Genre;
using BookManager.ViewModels.Publisher;
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
                    ImageUrl = b.ImageUrl,
                    CreatedByUserId = b.CreatedByUserId
                })
                .ToListAsync();
        }

        public async Task<BookViewModel?> GetByIdAsync(Guid id)
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
                ImageUrl = book.ImageUrl,
                CreatedByUserId = book.CreatedByUserId
            };
        }

        public async Task<IEnumerable<BookViewModel>> GetLatestAsync(int count)
        {
            return await _context.Books
                .OrderByDescending(b => b.Id)
                .Take(count)
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
                    ImageUrl = b.ImageUrl,
                    CreatedByUserId = b.CreatedByUserId
                })
                .ToListAsync();
        }

        public async Task<CreateBookViewModel> GetCreateModelAsync()
        {
            return new CreateBookViewModel
            {
                Authors = await GetAuthorsAsync(),
                Genres = await GetGenresAsync(),
                Publishers = await GetPublishersAsync()
            };
        }

        public async Task<EditBookViewModel?> GetEditModelAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            return new EditBookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                AuthorId = book.AuthorId,
                GenreId = book.GenreId,
                PublisherId = book.PublisherId,
                Authors = await GetAuthorsAsync(),
                Genres = await GetGenresAsync(),
                Publishers = await GetPublishersAsync()
            };
        }

        public async Task EditAsync(Guid id, EditBookViewModel model, string currentUserId, bool isAdmin)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return;

            if (book.CreatedByUserId != currentUserId && !isAdmin)
                return;

            book.Title = model.Title;
            book.Description = model.Description;
            book.ImageUrl = model.ImageUrl;
            book.AuthorId = model.AuthorId;
            book.GenreId = model.GenreId;
            book.PublisherId = model.PublisherId;

            await _context.SaveChangesAsync();
        }

        public async Task<List<AuthorDropdownViewModel>> GetAuthorsAsync()
        {
            return await _context.Authors
                .OrderBy(a => a.Name)
                .Select(a => new AuthorDropdownViewModel { Id = a.Id, Name = a.Name })
                .ToListAsync();
        }

        public async Task<List<GenreDropdownViewModel>> GetGenresAsync()
        {
            return await _context.Genres
                .OrderBy(g => g.Name)
                .Select(g => new GenreDropdownViewModel { Id = g.Id, Name = g.Name })
                .ToListAsync();
        }

        public async Task<List<PublisherDropdownViewModel>> GetPublishersAsync()
        {
            return await _context.Publishers
                .OrderBy(p => p.Name)
                .Select(p => new PublisherDropdownViewModel { Id = p.Id, Name = p.Name })
                .ToListAsync();
        }

        public async Task CreateAsync(CreateBookViewModel model, string createdByUserId)
        {
            Guid authorId;
            if (!string.IsNullOrWhiteSpace(model.NewAuthorName))
            {
                var newAuthor = new Author { Id = Guid.NewGuid(), Name = model.NewAuthorName.Trim(), CreatedByUserId = createdByUserId };
                _context.Authors.Add(newAuthor);
                await _context.SaveChangesAsync();
                authorId = newAuthor.Id;
            }
            else if (model.AuthorId.HasValue)
            {
                authorId = model.AuthorId.Value;
            }
            else throw new ArgumentException("Author is required.");

            Guid genreId;
            if (!string.IsNullOrWhiteSpace(model.NewGenreName))
            {
                var newGenre = new Genre { Id = Guid.NewGuid(), Name = model.NewGenreName.Trim() };
                _context.Genres.Add(newGenre);
                await _context.SaveChangesAsync();
                genreId = newGenre.Id;
            }
            else if (model.GenreId.HasValue)
            {
                genreId = model.GenreId.Value;
            }
            else throw new ArgumentException("Genre is required.");

            Guid publisherId;
            if (!string.IsNullOrWhiteSpace(model.NewPublisherName))
            {
                var newPublisher = new Publisher { Id = Guid.NewGuid(), Name = model.NewPublisherName.Trim() };
                _context.Publishers.Add(newPublisher);
                await _context.SaveChangesAsync();
                publisherId = newPublisher.Id;
            }
            else if (model.PublisherId.HasValue)
            {
                publisherId = model.PublisherId.Value;
            }
            else throw new ArgumentException("Publisher is required.");

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = model.Title.Trim(),
                Description = model.Description.Trim(),
                AuthorId = authorId,
                GenreId = genreId,
                PublisherId = publisherId,
                ImageUrl = model.ImageUrl,
                CreatedByUserId = createdByUserId
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task<BookViewModel?> GetDetailsByIdAsync(Guid id)
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
                ImageUrl = book.ImageUrl,
                CreatedByUserId = book.CreatedByUserId
            };
        }

        public async Task<BookAllViewModel> GetFilteredAsync(
            string? title,
            Guid? authorId,
            Guid? genreId,
            Guid? publisherId,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));

            if (authorId.HasValue && authorId != Guid.Empty)
                query = query.Where(b => b.AuthorId == authorId.Value);

            if (genreId.HasValue && genreId != Guid.Empty)
                query = query.Where(b => b.GenreId == genreId.Value);

            if (publisherId.HasValue && publisherId != Guid.Empty)
                query = query.Where(b => b.PublisherId == publisherId.Value);

            int totalBooks = await query.CountAsync();

            var books = await query
                .OrderBy(b => b.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Author = b.Author.Name,
                    Genre = b.Genre.Name,
                    Publisher = b.Publisher.Name,
                    ImageUrl = b.ImageUrl,
                    CreatedByUserId = b.CreatedByUserId
                })
                .ToListAsync();

            return new BookAllViewModel
            {
                Books = books,
                SearchTitle = title,
                SelectedAuthorId = authorId,
                SelectedGenreId = genreId,
                SelectedPublisherId = publisherId,
                Authors = await _context.Authors.OrderBy(a => a.Name)
                    .Select(a => new AuthorListViewModel { Id = a.Id, Name = a.Name }).ToListAsync(),
                Genres = await _context.Genres.OrderBy(g => g.Name)
                    .Select(g => new GenreViewModel { Id = g.Id, Name = g.Name }).ToListAsync(),
                Publishers = await _context.Publishers.OrderBy(p => p.Name)
                    .Select(p => new PublisherViewModel { Id = p.Id, Name = p.Name }).ToListAsync(),
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalBooks / (double)pageSize)
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
