using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BookManager.ViewModels.Publisher;

namespace BookManager.ViewModels.Book
{
    public class EditBookViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Display(Name = "Изображение (URL)")]
        public string? ImageUrl { get; set; }

        [Required]
        [Display(Name = "Автор")]
        public Guid AuthorId { get; set; }

        [Required]
        [Display(Name = "Жанр")]
        public Guid GenreId { get; set; }

        [Required]
        [Display(Name = "Издателство")]
        public Guid PublisherId { get; set; }

        public List<AuthorDropdownViewModel> Authors { get; set; } = new();
        public List<GenreDropdownViewModel> Genres { get; set; } = new();
        public List<PublisherDropdownViewModel> Publishers { get; set; } = new();
    }
}