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
    }
}