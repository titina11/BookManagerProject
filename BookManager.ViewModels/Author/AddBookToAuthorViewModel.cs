using System.ComponentModel.DataAnnotations;
using BookManager.ViewModels.Read;

namespace BookManager.ViewModels.Authors
{
    public class AddBookToAuthorViewModel
    {
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "Моля, изберете книга.")]
        public Guid SelectedBookId { get; set; }

        public List<BookDropdownViewModel> Books { get; set; } = new();
    }
}