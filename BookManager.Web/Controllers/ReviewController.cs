using BookManager.Data.Models;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid bookId)
    {
        var model = await _reviewService.GetCreateModelAsync(bookId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReviewViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _reviewService.CreateAsync(model, userId);
        return RedirectToAction("AllForBook", "Review", new { bookId = model.BookId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var model = await _reviewService.GetEditModelAsync(id);
        if (model == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (model.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditReviewViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!ModelState.IsValid) return View(model);

        await _reviewService.EditReviewAsync(id, model, userId, isAdmin);
        return RedirectToAction("AllForBook", "Review", new { bookId = model.BookId });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var model = await _reviewService.GetEditModelAsync(id);
        if (model == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (model.UserId != userId && !User.IsInRole("Admin")) return Forbid();

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDelete(Guid id)
    {
        var model = await _reviewService.GetEditModelAsync(id);
        if (model == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        await _reviewService.DeleteReviewAsync(id, userId, isAdmin);
        return RedirectToAction("AllForBook", new { bookId = model.BookId });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> AllForBook(Guid bookId)
    {
        var reviews = await _reviewService.GetReviewsForBookAsync(bookId);
        ViewBag.BookId = bookId;
        return View(reviews);
    }
}