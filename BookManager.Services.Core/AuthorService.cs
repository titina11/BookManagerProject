using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Author;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services.Core
{
    public class AuthorService : IAuthorService
    {
        private readonly BookManagerDbContext _context;

        public AuthorService(BookManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuthorViewModel>> GetAllAsync()
        {
            return await _context.Authors
                .Select(a => new AuthorViewModel
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();
        }

        public async Task<AuthorViewModel?> GetByIdAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return null;

            return new AuthorViewModel
            {
                Id = author.Id,
                Name = author.Name
            };
        }

        public async Task CreateAsync(CreateAuthorViewModel model)
        {
            var author = new Author
            {
                Name = model.Name
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(int id, EditAuthorViewModel model)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return;

            author.Name = model.Name;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
        }

        public async Task<DeleteAuthorViewModel?> GetDeleteByIdAsync(int id)
        {
            return await _context.Authors
                .Where(a => a.Id == id)
                .Select(a => new DeleteAuthorViewModel
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .FirstOrDefaultAsync();
        }
    }
}
