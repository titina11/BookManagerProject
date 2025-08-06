using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Publisher
{
    public class CreatePublisherViewModel
    {
        [Required(ErrorMessage = "Моля, въведете име на издателство.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Името трябва да е между 2 и 100 символа.")]
        public string Name { get; set; } = null!;
    }
}