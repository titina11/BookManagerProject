using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Publisher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PublishersController : Controller
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        public async Task<IActionResult> Index()
        {
            var publishers = await _publisherService.GetAllAsync();
            return View(publishers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePublisherViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePublisherViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var exists = await _publisherService.ExistsByNameAsync(model.Name);
            if (exists)
            {
                ModelState.AddModelError("Name", $"Издателство „{model.Name}“ вече съществува.");
                return View(model);
            }

            await _publisherService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var publisher = await _publisherService.GetByIdAsync(id);
            if (publisher == null) return NotFound();
            return View(publisher);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PublisherViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _publisherService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var publisher = await _publisherService.GetByIdAsync(id);
            if (publisher == null) return NotFound();
            return View(publisher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _publisherService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}

