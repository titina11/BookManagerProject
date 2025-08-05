using BookManager.ViewModels;
using BookManager.ViewModels.Book;
using BookManager.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;

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
        var bookServiceMock = new Mock<IBookService>();
        var loggerMock = new Mock<ILogger<HomeController>>();

        var controller = new HomeController(bookServiceMock.Object, loggerMock.Object);

        var context = new DefaultHttpContext();
        context.TraceIdentifier = "test-trace-id";

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        var activity = new Activity("Test");
        activity.Start();
        activity.Stop();
        Activity.Current = activity;

        var result = controller.Error();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.Model);

        Assert.False(string.IsNullOrEmpty(model.RequestId));
    }


    [Fact]
    public void Error_With404Code_ShouldReturn404View()
    {
        var result = _controller.Error(404);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("404", viewResult.ViewName);
    }

    [Fact]
    public void Error_With500Code_ShouldReturn500View()
    {
        var result = _controller.Error(500);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("500", viewResult.ViewName);
    }

    [Fact]
    public void Error_WithOtherCode_ShouldReturnDefaultErrorView()
    {
        var result = _controller.Error(123);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Error", viewResult.ViewName);
    }
}
