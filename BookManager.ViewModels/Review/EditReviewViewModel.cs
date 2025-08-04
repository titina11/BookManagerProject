using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Review
{
    public class EditReviewViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Съдържанието е задължително.")]
        [StringLength(2000, ErrorMessage = "Съдържанието трябва да е до 2000 символа.")]
        public string Content { get; set; } = null!;

        [Range(1, 5, ErrorMessage = "Оценката трябва да е между 1 и 5.")]
        public int Rating { get; set; }

        public Guid BookId { get; set; }

        public string UserId { get; set; } = null!;
    }
}