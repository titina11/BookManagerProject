using BookManager.Services.Core;
using BookManager.ViewModels.Author;
using BookManager.ViewModels.Authors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookManager.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("All");
        }

        public async Task<IActionResult> All()
        {
            var authors = await _authorService.GetAllAsync();

            ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.IsAdmin = User.IsInRole("Admin");

            return View(authors);
        }

        [HttpGet]
        public async Task<IActionResult> AddBook(Guid authorId)
        {
            var model = await _authorService.GetAddBookModelAsync(authorId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(AddBookToAuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Books = (await _authorService.GetAddBookModelAsync(model.AuthorId)).Books;
                return View(model);
            }

            await _authorService.AddBookToAuthorAsync(model);
            return RedirectToAction("All");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuthorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _authorService.CreateAsync(model, userId);

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();

            var model = new EditAuthorViewModel
            {
                Id = author.Id,
                Name = author.Name
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditAuthorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _authorService.UpdateAsync(model);
            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();

            var model = new DeleteAuthorViewModel
            {
                Id = author.Id,
                Name = author.Name
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();

            await _authorService.DeleteAsync(id);
            return RedirectToAction("All");
        }
    }
}
