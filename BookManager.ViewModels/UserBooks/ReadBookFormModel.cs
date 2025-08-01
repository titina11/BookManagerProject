
using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.Read
{
    public class ReadBookFormModel
    {
        [Required]
        public Guid BookId { get; set; }

        public List<BookDropdownViewModel> Books { get; set; } = new();

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Range(1, 10)]
        public int Rating { get; set; }
    }

    public class BookDropdownViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
    }
}