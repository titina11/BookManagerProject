using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Book
{
    public class CreateBookViewModel
    {
        public Guid AuthorId { get; set; }

        public Guid GenreId { get; set; }

        public Guid PublisherId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public IEnumerable<AuthorDropdownViewModel> Authors { get; set; } = new List<AuthorDropdownViewModel>();

        public IEnumerable<GenreDropdownViewModel> Genres { get; set; } = new List<GenreDropdownViewModel>();

        public IEnumerable<PublisherDropdownViewModel> Publishers { get; set; } = new List<PublisherDropdownViewModel>();
    }
}