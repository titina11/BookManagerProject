using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Genre;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services.Core
{
    public class GenreService : IGenreService
    {
        private readonly BookManagerDbContext _context;

        public GenreService(BookManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GenreViewModel>> GetAllAsync()
        {
            return await _context.Genres
                .Select(g => new GenreViewModel
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();
        }

        public async Task<GenreViewModel?> GetByIdAsync(Guid id)
        {
            var genre = await _context.Genres.FindAsync(id);
            return genre == null ? null : new GenreViewModel
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }

        public async Task CreateAsync(CreateGenreViewModel model)
        {
            _context.Genres.Add(new Genre
            {
                Id = Guid.NewGuid(),
                Name = model.Name
            });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Genres.AnyAsync(g => g.Name.ToLower() == name.ToLower());
        }


        public async Task UpdateAsync(GenreViewModel model)
        {
            var genre = await _context.Genres.FindAsync(model.Id);
            if (genre == null) return;

            genre.Name = model.Name;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return;

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }
    }
}