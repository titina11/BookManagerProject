using System.ComponentModel.DataAnnotations;

namespace BookManager.Data.Models
{
    public class Genre
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}