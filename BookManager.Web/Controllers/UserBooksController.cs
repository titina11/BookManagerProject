using BookManager.Data;
using BookManager.Data.Models;
using BookManager.ViewModels.UserBooks;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace BookManager.Controllers
{
    [Authorize]
    public class UserBooksController : Controller
    {
        private readonly BookManagerDbContext _context;

        public UserBooksController(BookManagerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Add(Guid bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return NotFound();

            var model = new UserBookListViewModel
            {
                BookId = bookId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Rating = 3
            };

            ViewBag.BookTitle = book.Title;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserBookListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var book = await _context.Books.FindAsync(model.BookId);
                ViewBag.BookTitle = book?.Title ?? "";
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userBook = new UserBook
            {
                Id = Guid.NewGuid(),
                UserId = userId!,
                BookId = model.BookId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Rating = model.Rating
            };

            _context.UserBooks.Add(userBook);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyBooks");
        }

        [HttpGet]
        public async Task<IActionResult> MyBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var books = await _context.UserBooks
                .Where(ub => ub.UserId == userId)
                .Include(ub => ub.Book)
                .Select(ub => new UserBookListViewModel
                {
                    Title = ub.Book.Title,
                    StartDate = ub.StartDate,
                    EndDate = ub.EndDate,
                    Rating = ub.Rating,
                })
                .ToListAsync();

            return View(books);
        }
    }
}
