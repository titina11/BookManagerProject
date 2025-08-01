using BookManager.Data;
using BookManager.Data.Models;
using BookManager.ViewModels.Author;
using BookManager.ViewModels.Authors;
using BookManager.ViewModels.Book;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AuthorService : IAuthorService
{
    private readonly BookManagerDbContext _context;

    public AuthorService(BookManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuthorListViewModel>> GetAllAsync()
    {
        return await _context.Authors
            .Include(a => a.Books)
            .Select(a => new AuthorListViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Books = a.Books
                    .Select(b => new BookShortViewModel
                    {
                        Id = b.Id,
                        Title = b.Title
                    }).ToList()
            })
            .ToListAsync();
    }
    public async Task<AddBookToAuthorViewModel> GetAddBookModelAsync(Guid authorId)
    {
        var books = await _context.Books
            .Where(b => b.AuthorId != authorId)
            .Select(b => new BookDropdownViewModel
            {
                Id = b.Id,
                Title = b.Title
            })
            .ToListAsync();

        return new AddBookToAuthorViewModel
        {
            AuthorId = authorId,
            Books = books
        };
    }

    public async Task AddBookToAuthorAsync(AddBookToAuthorViewModel model)
    {
        var book = await _context.Books.FindAsync(model.SelectedBookId);
        if (book == null) return;

        book.AuthorId = model.AuthorId;
        await _context.SaveChangesAsync();
    }

    public async Task CreateAsync(CreateAuthorViewModel model)
    {
        var author = new Author { Id = Guid.NewGuid(), Name = model.Name };
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
    }
}
