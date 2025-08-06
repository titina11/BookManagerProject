using BookManager.ViewModels.Publisher;

namespace BookManager.Services.Core.Contracts
{
    public interface IPublisherService
    {
        Task<List<PublisherDropdownViewModel>> GetAllAsync();

        Task<List<PublisherViewModel>> GetAllDetailedAsync(); 

        Task<PublisherViewModel?> GetByIdAsync(Guid id);

        Task<bool> ExistsByNameAsync(string name);

        Task CreateAsync(CreatePublisherViewModel model);

        Task UpdateAsync(PublisherViewModel model);

        Task DeleteAsync(Guid id);
    }
}