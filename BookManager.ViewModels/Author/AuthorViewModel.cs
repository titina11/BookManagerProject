using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Author
{
    public class AuthorViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

    }
}