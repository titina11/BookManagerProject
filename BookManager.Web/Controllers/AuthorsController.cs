using BookManager.Services.Core.Contracts;
using BookManager.ViewModels;
using BookManager.ViewModels.Author;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.Web.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET: Author
        public async Task<IActionResult> Index()
        {
            var authors = await _authorService.GetAllAsync();
            return View(authors);
        }


        // GET: Author Details
        public async Task<IActionResult> Details(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();

            return View(author);

        }

        // GET: Author Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Author Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _authorService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: Author Edit
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            var model = new AuthorViewModel
            {
                Id = author.Id,
                Name = author.Name
            };

            return View(model);
        }

        // POST: Author Edit
        [HttpPost]
        public async Task<IActionResult> Edit(AuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var editModel = new EditAuthorViewModel
            {
                Name = model.Name
            };

            await _authorService.EditAsync(model.Id, editModel);
            return RedirectToAction(nameof(Index));
        }

        // GET: Author Delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return View(author); 
        }

        // POST: Author Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _authorService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
