namespace BookManager.Data.Models
{
    public class Publisher
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!; 

        public string? Description { get; set; } 

        public ICollection<Book> Books { get; set; } = new HashSet<Book>();
    }
}