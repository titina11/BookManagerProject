using BookManager.ViewModels.Genre;

namespace BookManager.Services.Core.Contracts
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreViewModel>> GetAllAsync();
        Task<GenreViewModel?> GetByIdAsync(Guid id);
        Task CreateAsync(GenreViewModel model);
        Task UpdateAsync(GenreViewModel model);
        Task DeleteAsync(Guid id);
    }
}