using BookManager.Data;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Publisher;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services
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
            var publishers = await this.dbContext.Publishers
                .Select(p => new PublisherDropdownViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();

            return publishers;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var normalized = name.Trim().ToLower();

            return await dbContext.Publishers
                .AnyAsync(p => p.Name.ToLower().Trim() == normalized);
        }


    }
}