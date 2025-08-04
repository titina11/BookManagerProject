using BookManager.ViewModels.Review;

namespace BookManager.Services.Core.Contracts
{
    public interface IReviewService
    {
        Task<CreateReviewViewModel> GetCreateModelAsync(Guid bookId);
        Task CreateAsync(CreateReviewViewModel model, string userId);
        Task<EditReviewViewModel?> GetEditModelAsync(Guid id);
        Task EditReviewAsync(Guid id, EditReviewViewModel model, string currentUserId, bool isAdmin);
        Task DeleteReviewAsync(Guid id, string currentUserId, bool isAdmin);
        Task<List<ReviewViewModel>> GetReviewsForBookAsync(Guid bookId);
    }
}