using System.ComponentModel.DataAnnotations;

namespace BookManager.Data.Models
{
    public class Publisher
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; } 

        public ICollection<Book> Books { get; set; } = new HashSet<Book>();
    }
}