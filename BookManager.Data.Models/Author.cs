using System.ComponentModel.DataAnnotations;

namespace BookManager.Data.Models
{
    public class Author
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}