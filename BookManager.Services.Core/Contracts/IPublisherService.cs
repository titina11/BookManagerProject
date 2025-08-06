using BookManager.ViewModels.Publisher;

namespace BookManager.Services.Core.Contracts
{
    public interface IPublisherService
    {
        Task<List<PublisherDropdownViewModel>> GetAllAsync();

        Task<bool> ExistsByNameAsync(string name);


    }
}