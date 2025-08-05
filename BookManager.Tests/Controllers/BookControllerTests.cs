using BookManager.ViewModels.Book;
using BookManager.ViewModels.Books;
using BookManager.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System.Security.Claims;

namespace BookManager.Tests.Controllers;

public class BookControllerTests
{
    private readonly Mock<IBookService> _bookServiceMock;
    private readonly BookController _controller;

    public BookControllerTests()
    {
        _bookServiceMock = new Mock<IBookService>();
        _controller = new BookController(_bookServiceMock.Object);

        var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = testUser }
        };
    }

    [Fact]
    public async Task All_ShouldReturnViewWithFilteredBooksAndSetViewBag()
    {
        var filteredModel = new BookFilterViewModel
        {
            Books = new List<BookViewModel>
            {
                new BookViewModel { Id = Guid.NewGuid(), Title = "Test Book" }
            },
            Authors = new List<AuthorDropdownViewModel>(),
            Genres = new List<GenreDropdownViewModel>(),
            Publishers = new List<PublisherDropdownViewModel>()
        };

        _bookServiceMock
            .Setup(s => s.GetFilteredAsync(null, null, null, null))
            .ReturnsAsync(filteredModel);

        var controller = new BookController(_bookServiceMock.Object);

        var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = testUser }
        };

        var result = await controller.All(null, null, null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BookFilterViewModel>(viewResult.Model);
        Assert.Single(model.Books);

        Assert.Equal("test-user-id", controller.ViewBag.CurrentUserId);
        Assert.True((bool)controller.ViewBag.IsAdmin);
    }

    [Fact]
    public async Task Create_Get_ShouldReturnViewWithModel()
    {
        var model = new CreateBookViewModel
        {
            Authors = new List<AuthorDropdownViewModel>
            {
                new AuthorDropdownViewModel { Id = Guid.NewGuid(), Name = "Author 1" }
            },
            Genres = new List<GenreDropdownViewModel>
            {
                new GenreDropdownViewModel { Id = Guid.NewGuid(), Name = "Genre 1" }
            },
            Publishers = new List<PublisherDropdownViewModel>
            {
                new PublisherDropdownViewModel { Id = Guid.NewGuid(), Name = "Publisher 1" }
            }
        };

        _bookServiceMock
            .Setup(s => s.GetCreateModelAsync())
            .ReturnsAsync(model);

        var result = await _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsAssignableFrom<CreateBookViewModel>(viewResult.Model);

        Assert.Single(returnedModel.Authors);
        Assert.Single(returnedModel.Genres);
        Assert.Single(returnedModel.Publishers);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnView_WhenModelIsInvalid()
    {
        var model = new CreateBookViewModel
        {
            Title = "", 
            AuthorId = null, 
            GenreId = Guid.NewGuid(),
            PublisherId = Guid.NewGuid()
        };

        _controller.ModelState.AddModelError("Title", "Required");

        _bookServiceMock.Setup(s => s.GetAuthorsAsync())
            .ReturnsAsync(new List<AuthorDropdownViewModel> { new() { Id = Guid.NewGuid(), Name = "A1" } });
        _bookServiceMock.Setup(s => s.GetGenresAsync())
            .ReturnsAsync(new List<GenreDropdownViewModel> { new() { Id = Guid.NewGuid(), Name = "G1" } });
        _bookServiceMock.Setup(s => s.GetPublishersAsync())
            .ReturnsAsync(new List<PublisherDropdownViewModel> { new() { Id = Guid.NewGuid(), Name = "P1" } });

        var result = await _controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsAssignableFrom<CreateBookViewModel>(viewResult.Model);

        Assert.NotEmpty(returnedModel.Authors);
        Assert.NotEmpty(returnedModel.Genres);
        Assert.NotEmpty(returnedModel.Publishers);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnUnauthorized_WhenUserNotLoggedIn()
    {
        var model = new CreateBookViewModel
        {
            Title = "Book",
            AuthorId = Guid.NewGuid(),
            GenreId = Guid.NewGuid(),
            PublisherId = Guid.NewGuid()
        };

        var controllerNoUser = new BookController(_bookServiceMock.Object);
        controllerNoUser.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() 
        };

        var result = await controllerNoUser.Create(model);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Create_Post_ShouldCallServiceAndRedirect_WhenModelIsValid()
    {
        var model = new CreateBookViewModel
        {
            Title = "Test Book",
            AuthorId = Guid.NewGuid(),
            GenreId = Guid.NewGuid(),
            PublisherId = Guid.NewGuid()
        };

        var result = await _controller.Create(model);

        _bookServiceMock.Verify(s => s.CreateAsync(model, "test-user-id"), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", redirectResult.ActionName);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnNotFound_WhenEditModelIsNull()
    {
        var bookId = Guid.NewGuid();

        _bookServiceMock.Setup(s => s.GetEditModelAsync(bookId))
            .ReturnsAsync((EditBookViewModel?)null);

        var result = await _controller.Edit(bookId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnForbid_WhenUserIsNotOwnerOrAdmin()
    {
        var bookId = Guid.NewGuid();

        var mockService = new Mock<IBookService>();
        mockService.Setup(s => s.GetEditModelAsync(bookId))
            .ReturnsAsync(new EditBookViewModel());

        mockService.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(new BookViewModel
            {
                Id = bookId,
                CreatedByUserId = "another-user-id" 
            });

        var controller = new BookController(mockService.Object);

        var fakeUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id") 
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = fakeUser }
        };

        var result = await controller.Edit(bookId);

        Assert.IsType<ForbidResult>(result); 
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnView_WhenUserIsOwner()
    {
        var bookId = Guid.NewGuid();

        _bookServiceMock.Setup(s => s.GetEditModelAsync(bookId))
            .ReturnsAsync(new EditBookViewModel { Title = "Editable Book" });

        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(new BookViewModel
            {
                Id = bookId,
                CreatedByUserId = "test-user-id"
            });

        var result = await _controller.Edit(bookId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<EditBookViewModel>(viewResult.Model);
        Assert.Equal("Editable Book", model.Title);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnForbid_WhenBookIsNull()
    {
        var bookId = Guid.NewGuid();
        var model = new EditBookViewModel { Title = "Book" };

        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync((BookViewModel?)null);

        var result = await _controller.Edit(bookId, model);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnForbid_WhenUserIsNotOwnerOrAdmin()
    {
        var bookId = Guid.NewGuid();
        var model = new EditBookViewModel { Title = "Book" };

        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(new BookViewModel
            {
                Id = bookId,
                CreatedByUserId = "another-user-id"
            });

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.Edit(bookId, model);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnView_WhenModelStateIsInvalid()
    {
        var bookId = Guid.NewGuid();
        var model = new EditBookViewModel
        {
            Title = "", 
        };

        _controller.ModelState.AddModelError("Title", "Required");

        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(new BookViewModel
            {
                Id = bookId,
                CreatedByUserId = "test-user-id"
            });

        _bookServiceMock.Setup(s => s.GetAuthorsAsync())
            .ReturnsAsync(new List<AuthorDropdownViewModel> { new() { Id = Guid.NewGuid(), Name = "Author" } });

        _bookServiceMock.Setup(s => s.GetGenresAsync())
            .ReturnsAsync(new List<GenreDropdownViewModel> { new() { Id = Guid.NewGuid(), Name = "Genre" } });

        _bookServiceMock.Setup(s => s.GetPublishersAsync())
            .ReturnsAsync(new List<PublisherDropdownViewModel> { new() { Id = Guid.NewGuid(), Name = "Pub" } });

        var result = await _controller.Edit(bookId, model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsAssignableFrom<EditBookViewModel>(viewResult.Model);
        Assert.NotEmpty(returnedModel.Authors);
        Assert.NotEmpty(returnedModel.Genres);
        Assert.NotEmpty(returnedModel.Publishers);
    }

    [Fact]
    public async Task Edit_Post_ShouldCallEditAndRedirect_WhenModelIsValidAndUserIsOwner()
    {
        var bookId = Guid.NewGuid();
        var model = new EditBookViewModel
        {
            Title = "Updated Book",
            AuthorId = Guid.NewGuid(),
            GenreId = Guid.NewGuid(),
            PublisherId = Guid.NewGuid()
        };

        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(new BookViewModel
            {
                Id = bookId,
                CreatedByUserId = "test-user-id"
            });

        var result = await _controller.Edit(bookId, model);

        _bookServiceMock.Verify(s => s.EditAsync(bookId, model, "test-user-id", true), Times.Once);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", redirect.ActionName);
    }

    [Fact]
    public async Task Delete_ShouldReturnForbid_WhenBookIsNull()
    {
        var bookId = Guid.NewGuid();
        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync((BookViewModel?)null);

        var result = await _controller.Delete(bookId);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnForbid_WhenUserIsNotOwnerOrAdmin()
    {
        var bookId = Guid.NewGuid();

        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(new BookViewModel
            {
                Id = bookId,
                CreatedByUserId = "another-user-id"
            });

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.Delete(bookId);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnView_WhenUserIsOwner()
    {
        var bookId = Guid.NewGuid();
        var userId = "test-user-id";

        var book = new BookViewModel
        {
            Id = bookId,
            Title = "Book Title",
            CreatedByUserId = userId
        };

        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(book);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.Delete(bookId);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(book, viewResult.Model);
    }

    [Fact]
    public async Task ConfirmDelete_ShouldDeleteBook_WhenUserIsOwner()
    {
        var bookId = Guid.NewGuid();
        var userId = "user-xyz";

        var book = new BookViewModel
        {
            Id = bookId,
            CreatedByUserId = userId
        };

        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(book);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.ConfirmDelete(bookId);

        _bookServiceMock.Verify(s => s.DeleteAsync(bookId), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(_controller.All), redirect.ActionName);
    }

    [Fact]
    public async Task ConfirmDelete_ShouldReturnForbid_WhenUserIsNotOwnerOrAdmin()
    {
        var bookId = Guid.NewGuid();
        var book = new BookViewModel
        {
            Id = bookId,
            CreatedByUserId = "creator-id"
        };

        _bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(book);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "not-the-creator"),
            new Claim(ClaimTypes.Role, "User")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.ConfirmDelete(bookId);

        Assert.IsType<ForbidResult>(result);
        _bookServiceMock.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        var bookId = Guid.NewGuid();

        _bookServiceMock.Setup(s => s.GetDetailsByIdAsync(bookId))
            .ReturnsAsync((BookViewModel?)null);

        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/Book/All");

        _controller.Url = urlHelperMock.Object;

        var result = await _controller.Details(bookId, null);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ShouldReturnView_WhenBookExists()
    {
        var bookId = Guid.NewGuid();
        var bookViewModel = new BookViewModel
        {
            Id = bookId,
            Title = "Test Book"
        };

        _bookServiceMock.Setup(s => s.GetDetailsByIdAsync(bookId))
            .ReturnsAsync(bookViewModel);

        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/Book/All");

        _controller.Url = urlHelperMock.Object;

        var result = await _controller.Details(bookId, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BookViewModel>(viewResult.Model);
        Assert.Equal("Test Book", model.Title);
        Assert.Equal(bookId, model.Id);
    }

}