using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Genre
{
    public class EditGenreViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [StringLength(50, ErrorMessage = "Името не може да е по-дълго от 50 символа.")]
        public string Name { get; set; } = null!;
    }
}