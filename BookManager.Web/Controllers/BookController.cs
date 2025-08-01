using BookManager.Services.Core;
using BookManager.ViewModels.Book;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

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

            await _bookService.CreateAsync(model);
            return RedirectToAction(nameof(All));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _bookService.GetEditModelAsync(id);
            if (model == null) return NotFound();

            return View(model); 
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EditBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Authors = await _bookService.GetAuthorsAsync();
                model.Genres = await _bookService.GetGenresAsync();
                model.Publishers = await _bookService.GetPublishersAsync();
                return View("All", model);
            }

            await _bookService.EditAsync(id, model);
                return RedirectToAction(nameof(All));
        }

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

        public async Task<IActionResult> Delete(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            await _bookService.DeleteAsync(id);
            return RedirectToAction(nameof(All));
        }
    }
}
