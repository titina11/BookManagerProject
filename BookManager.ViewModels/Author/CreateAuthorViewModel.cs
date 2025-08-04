using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Author
{
    public class CreateAuthorViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

    }
}