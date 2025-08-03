using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Review
{
    public class CreateReviewViewModel
    {
        public Guid BookId { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "Съдържанието трябва да е до 2000 символа.")]
        public string Content { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        public string BookTitle { get; set; } = string.Empty;
    }
}

