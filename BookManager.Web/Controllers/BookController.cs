using BookManager.Services.Core;
using BookManager.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookManager.Data.Models;

namespace BookManager.Web.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> All(string? title, Guid? authorId, Guid? genreId, Guid? publisherId)
        {
            var model = await _bookService.GetFilteredAsync(title, authorId, genreId, publisherId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _bookService.GetCreateModelAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Authors = await _bookService.GetAuthorsAsync();
                model.Genres = await _bookService.GetGenresAsync();
                model.Publishers = await _bookService.GetPublishersAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.CreatedByUserId = userId;

            await _bookService.CreateAsync(model);
            return RedirectToAction(nameof(All));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _bookService.GetEditModelAsync(id);
            if (model == null) return NotFound();

            var book = await _bookService.GetByIdAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (book == null || (book.CreatedByUserId != userId && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EditBookViewModel model)
        {
            var book = await _bookService.GetByIdAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (book == null || (book.CreatedByUserId != userId && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                model.Authors = await _bookService.GetAuthorsAsync();
                model.Genres = await _bookService.GetGenresAsync();
                model.Publishers = await _bookService.GetPublishersAsync();
                return View(model);
            }

            await _bookService.EditAsync(id, model);
            return RedirectToAction(nameof(All));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? "Books";

            var model = await _bookService.GetDetailsByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (book == null || (book.CreatedByUserId != userId && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (book == null || (book.CreatedByUserId != userId && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            await _bookService.DeleteAsync(id);
            return RedirectToAction(nameof(All));
        }
    }
}
