using BookManager.Services.Core;
using BookManager.ViewModels.Author;
using BookManager.ViewModels.Authors;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IAuthorBookService _authorBookService;

        public AuthorsController(IAuthorService authorService, IAuthorBookService authorBookService)
        {
            _authorService = authorService;
            _authorBookService = authorBookService;
        }

        public async Task<IActionResult> All()
        {
            var authors = await _authorService.GetAllAsync();
            return View(authors);
        }

        [HttpGet]
        public async Task<IActionResult> AddBook(Guid authorId)
        {
            var model = await _authorBookService.GetAddBookModelAsync(authorId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(AddBookToAuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Books = (await _authorBookService.GetAddBookModelAsync(model.AuthorId)).Books;
                return View(model);
            }

            await _authorBookService.AddBookToAuthorAsync(model);
            return RedirectToAction("All");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAuthorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _authorService.CreateAsync(model);
            return RedirectToAction(nameof(All));
        }
    }
}