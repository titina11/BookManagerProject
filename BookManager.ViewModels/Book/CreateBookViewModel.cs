using System.ComponentModel.DataAnnotations;
using BookManager.ViewModels.Publisher;

namespace BookManager.ViewModels.Book;

public class CreateBookViewModel
{
    [Display(Name = "Автор")]
    public Guid? AuthorId { get; set; }

    [Display(Name = "Жанр")]
    public Guid? GenreId { get; set; }

    [Display(Name = "Издателство")]
    public Guid? PublisherId { get; set; }

    [Display(Name = "Нов автор (ако не е в списъка)")]
    public string? NewAuthorName { get; set; }

    [Display(Name = "Нов жанр (ако не е в списъка)")]
    public string? NewGenreName { get; set; }

    [Display(Name = "Ново издателство (ако не е в списъка)")]
    public string? NewPublisherName { get; set; }

    [Required(ErrorMessage = "Моля, въведете заглавие.")]
    [StringLength(100, ErrorMessage = "Заглавието не може да е по-дълго от 100 символа.")]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public List<AuthorDropdownViewModel> Authors { get; set; } = new();
    public List<GenreDropdownViewModel> Genres { get; set; } = new();
    public List<PublisherDropdownViewModel> Publishers { get; set; } = new();
}