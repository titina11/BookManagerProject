using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Author
{
    public class AuthorViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

    }
}