using System.ComponentModel.DataAnnotations;

namespace BookManager.Data.Models
{
    public class Book
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public Guid AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public Guid PublisherId { get; set; }    
        public Publisher Publisher { get; set; } = null!;

        public Guid GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        public string? ImageUrl { get; set; }
        public ICollection<UserBook> UserBooks { get; set; } = new HashSet<UserBook>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}