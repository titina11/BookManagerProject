using BookManager.Services.Core.Contracts;
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
        public async Task<IActionResult> All()
        {
            var books = await _bookService.GetAllAsync();
            return View(books);
        }

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
                var filledModel = await _bookService.GetCreateModelAsync();
                model.Authors = filledModel.Authors;
                model.Genres = filledModel.Genres;
                model.Publishers = filledModel.Publishers;

                return View(model);
            }

            await _bookService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

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
                var filledModel = await _bookService.GetEditModelAsync(id);
                if (filledModel == null) return NotFound();

                model.Authors = filledModel.Authors;
                model.Genres = filledModel.Genres;
                model.Publishers = filledModel.Publishers;

                return View(model);
            }

            await _bookService.EditAsync(id, model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _bookService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
