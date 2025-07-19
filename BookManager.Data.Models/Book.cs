using System.ComponentModel.DataAnnotations;

namespace BookManager.Data.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public int PublisherId { get; set; }    
        public Publisher Publisher { get; set; } = null!;

        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        public string? ImageUrl { get; set; }
        public ICollection<UserBook> UserBooks { get; set; } = new HashSet<UserBook>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}