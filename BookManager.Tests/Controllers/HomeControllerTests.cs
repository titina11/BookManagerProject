using BookManager.ViewModels;
using BookManager.ViewModels.Book;
using BookManager.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookManager.Tests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<IBookService> _bookServiceMock;
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _bookServiceMock = new Mock<IBookService>();
        _loggerMock = new Mock<ILogger<HomeController>>();

        _controller = new HomeController(_bookServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithLatestBooks()
    {
        var books = new List<BookViewModel>
        {
            new BookViewModel { Id = Guid.NewGuid(), Title = "Book 1" },
            new BookViewModel { Id = Guid.NewGuid(), Title = "Book 2" },
            new BookViewModel { Id = Guid.NewGuid(), Title = "Book 3" }
        };

        _bookServiceMock
            .Setup(s => s.GetLatestAsync(3))
            .ReturnsAsync(books);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<BookViewModel>>(viewResult.Model);

        Assert.Equal(3, model.Count());
    }

    [Fact]
    public void Privacy_ShouldReturnView()
    {
        var result = _controller.Privacy();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ShouldReturnViewWithErrorViewModel()
    {
        var requestId = "test-id";
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = requestId;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = _controller.Error();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);

        Assert.Equal(requestId, model.RequestId);
    }
}