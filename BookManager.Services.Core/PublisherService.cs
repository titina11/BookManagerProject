
using BookManager.Data.Models;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Publisher;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services.Core
{
    public class PublisherService : IPublisherService
    {
        private readonly BookManagerDbContext dbContext;

        public PublisherService(BookManagerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<PublisherDropdownViewModel>> GetAllAsync()
        {
            return await dbContext.Publishers
                .Select(p => new PublisherDropdownViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }

        public async Task<List<PublisherViewModel>> GetAllDetailedAsync()
        {
            return await dbContext.Publishers
                .Select(p => new PublisherViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }

        public async Task<PublisherViewModel?> GetByIdAsync(Guid id)
        {
            var publisher = await dbContext.Publishers.FindAsync(id);

            if (publisher == null)
                return null;

            return new PublisherViewModel
            {
                Id = publisher.Id,
                Name = publisher.Name
            };
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var normalized = name.Trim().ToLower();

            return await dbContext.Publishers
                .AnyAsync(p => p.Name.ToLower().Trim() == normalized);
        }

        public async Task CreateAsync(CreatePublisherViewModel model)
        {
            var publisher = new Publisher
            {
                Id = Guid.NewGuid(),
                Name = model.Name
            };

            dbContext.Publishers.Add(publisher);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(PublisherViewModel model)
        {
            var publisher = await dbContext.Publishers.FindAsync(model.Id);

            if (publisher != null)
            {
                publisher.Name = model.Name;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var publisher = await dbContext.Publishers.FindAsync(id);

            if (publisher != null)
            {
                dbContext.Publishers.Remove(publisher);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
