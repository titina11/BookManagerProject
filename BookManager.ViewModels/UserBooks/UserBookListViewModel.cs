using System;
using System.ComponentModel.DataAnnotations;

namespace BookManager.ViewModels.UserBooks
{
    public class UserBookListViewModel
    {
        public Guid BookId { get; set; }

        public string Title { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Начална дата")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Крайна дата")]
        public DateTime EndDate { get; set; }

        [Range(1, 5)]
        [Display(Name = "Рейтинг (1–5)")]
        public int Rating { get; set; }

        [Display(Name = "Дни четене")]
        public int DaysRead { get; set; }
    }
}