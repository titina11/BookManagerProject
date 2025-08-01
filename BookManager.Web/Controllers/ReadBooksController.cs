using BookManager.Data.Models;
using BookManager.ViewModels.Read;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class ReadBooksController : Controller
{
    private readonly IReadBookService _readBookService;
    private readonly IBookService _bookService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReadBooksController(IReadBookService readBookService, IBookService bookService, UserManager<ApplicationUser> userManager)
    {
        _readBookService = readBookService;
        _bookService = bookService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(); 
        }
        var model = await _readBookService.GetByUserAsync(userId);
        return View(model);
    }

    public async Task<IActionResult> Add()
    {
        var books = await _bookService.GetAllAsync();
        var formModel = new ReadBookFormModel
        {
            Books = books.Select(b => new BookDropdownViewModel { Id = b.Id, Title = b.Title }).ToList()
        };
        return View(formModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(ReadBookFormModel model)
    {
        if (!ModelState.IsValid)
        {
            var books = await _bookService.GetAllAsync();
            model.Books = books.Select(b => new BookDropdownViewModel { Id = b.Id, Title = b.Title }).ToList();
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        await _readBookService.AddAsync(userId, model);
        return RedirectToAction(nameof(Index));
    }
}