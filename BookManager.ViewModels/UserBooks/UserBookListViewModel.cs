using System;
using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.UserBooks
{
    public class UserBookListViewModel
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "Моля въведете начална дата")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Моля въведете крайна дата")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Range(1, 5, ErrorMessage = "Рейтингът трябва да е между 1 и 5")]
        public int Rating { get; set; }

        public string? Title { get; set; }

        public int DaysRead => (EndDate - StartDate).Days;

    }
}