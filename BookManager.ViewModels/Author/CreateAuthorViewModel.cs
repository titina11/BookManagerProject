using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Authors
{
    public class CreateAuthorViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        public string CreatedByUserId { get; set; } = null!;
    }
}