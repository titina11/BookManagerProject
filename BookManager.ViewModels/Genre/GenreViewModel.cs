using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Genre
{
    public class GenreViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Полето 'Жанр' е задължително.")]
        [StringLength(50, ErrorMessage = "Името на жанра трябва да е до 50 символа.")]
        [Display(Name = "Жанр")]
        public string Name { get; set; } = null!;
    }
}