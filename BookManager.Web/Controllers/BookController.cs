using BookManager.Data.Models;
using BookManager.Services.Core;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookManager.Web.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IPublisherService _publisherService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;

        public BookController(
            IBookService bookService,
            IPublisherService publisherService,
            IAuthorService authorService,
            IGenreService genreService)
        {
            _bookService = bookService;
            _publisherService = publisherService;
            _authorService = authorService;
            _genreService = genreService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> All(string? title, Guid? authorId, Guid? genreId, Guid? publisherId, int page = 1)
        {
            int pageSize = 5;

            var model = await _bookService.GetFilteredAsync(title, authorId, genreId, publisherId, page, pageSize);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.IsAdmin = isAdmin;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _bookService.GetCreateModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            if (!ModelState.IsValid || model.AuthorId == null || model.GenreId == null || model.PublisherId == null)
            {

                if (model.AuthorId == null)
                    ModelState.AddModelError(nameof(model.AuthorId), "Моля, изберете автор или добавете нов.");
                if (model.GenreId == null)
                    ModelState.AddModelError(nameof(model.GenreId), "Моля, изберете жанр или добавете нов.");
                if (model.PublisherId == null)
                    ModelState.AddModelError(nameof(model.PublisherId), "Моля, изберете издателство или добавете нов.");

                if (!string.IsNullOrWhiteSpace(model.NewPublisherName))
                {
                    bool exists = await _publisherService.ExistsByNameAsync(model.NewPublisherName);
                    if (exists)
                    {
                        ModelState.AddModelError("NewPublisherName", "Издателството вече съществува.");
                        return View(model);
                    }
                }

                model.Authors = await _bookService.GetAuthorsAsync();
                model.Genres = await _bookService.GetGenresAsync();
                model.Publishers = await _bookService.GetPublishersAsync();

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _bookService.CreateAsync(model, userId);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditBookViewModel model)
        {
            var book = await _bookService.GetByIdAsync(id);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var isAdmin = User.IsInRole("Admin");                               

            if (book == null || (book.CreatedByUserId != currentUserId && !isAdmin))
                return Forbid();

            if (!ModelState.IsValid)
            {
                model.Authors = await _bookService.GetAuthorsAsync();
                model.Genres = await _bookService.GetGenresAsync();
                model.Publishers = await _bookService.GetPublishersAsync();
                return View(model);
            }

            await _bookService.EditAsync(id, model, currentUserId, isAdmin); 

            return RedirectToAction(nameof(All));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? Url.Action(nameof(All));

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
        [ValidateAntiForgeryToken]
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
