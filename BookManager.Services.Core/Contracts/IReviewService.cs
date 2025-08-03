using BookManager.ViewModels;
using BookManager.ViewModels.Review;

public interface IReviewService
{
    Task<CreateReviewViewModel> GetCreateModelAsync(Guid bookId);
    Task CreateAsync(CreateReviewViewModel model, string userId);
    Task<IEnumerable<ReviewViewModel>> GetByBookIdAsync(Guid bookId);
    Task AddReviewAsync(CreateReviewViewModel model, string userId);
    Task<List<ReviewViewModel>> GetReviewsForBookAsync(Guid bookId);
}