using System.Diagnostics;
using BookManager.ViewModels;
using BookManager.ViewModels.Book; 
using BookManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookManager.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IBookService bookService, ILogger<HomeController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetLatestAsync(3);
            return View(books);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Home/Error/{code:int}")]
        public IActionResult Error(int code)
        {
            if (code == 404) return View("404");
            if (code == 500) return View("500");

            return View("Error");
        }
    }
}