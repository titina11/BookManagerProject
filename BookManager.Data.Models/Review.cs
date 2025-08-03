using System.ComponentModel.DataAnnotations;

namespace BookManager.Data.Models
{
    public class Review
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public Guid BookId { get; set; }
        public Book Book { get; set; } = null!;
    }

}