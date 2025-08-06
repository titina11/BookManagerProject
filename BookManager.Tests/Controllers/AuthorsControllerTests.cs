using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BookManager.Controllers;
using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Author;
using BookManager.ViewModels.Authors;

namespace BookManager.Tests.Controllers;

public class AuthorsControllerTests
{
    private readonly Mock<IAuthorService> _authorServiceMock;
    private readonly AuthorsController _controller;

    public AuthorsControllerTests()
    {
        _authorServiceMock = new Mock<IAuthorService>();

        _controller = new AuthorsController(_authorServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task All_ShouldReturnViewWithAuthors()
    {
        var authors = new List<AuthorListViewModel>
        {
            new AuthorListViewModel { Id = Guid.NewGuid(), Name = "Author 1" },
            new AuthorListViewModel { Id = Guid.NewGuid(), Name = "Author 2" }
        };

        _authorServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(authors);

        var controller = new AuthorsController(_authorServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await controller.All();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<AuthorListViewModel>>(viewResult.Model);
        Assert.Equal(2, model.Count);

        Assert.Equal("test-user-id", controller.ViewBag.CurrentUserId);
        Assert.True((bool)controller.ViewBag.IsAdmin);
    }

    [Fact]
    public void Create_Get_ShouldReturnView()
    {
        var result = _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.Model); 
    }

    [Fact]
    public async Task Create_Post_ValidModel_ShouldCreateAuthorAndRedirect()
    {
        var model = new CreateAuthorViewModel { Name = "New Author" };

        var result = await _controller.Create(model);

        _authorServiceMock.Verify(s =>
            s.CreateAsync(model, "test-user-id"), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", redirectResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ShouldReturnViewWithModel()
    {
        var model = new CreateAuthorViewModel { Name = "" }; 
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsAssignableFrom<CreateAuthorViewModel>(viewResult.Model);
        Assert.Equal(model, returnedModel);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnError_WhenAuthorAlreadyExists()
    {
        var authorServiceMock = new Mock<IAuthorService>();
        var controller = new AuthorsController(authorServiceMock.Object);

        var model = new CreateAuthorViewModel { Name = "Existing Author" };

        authorServiceMock
            .Setup(s => s.ExistsByNameAsync("Existing Author"))
            .ReturnsAsync(true); 

        var result = await controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<CreateAuthorViewModel>(viewResult.Model);

        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey("Name"));
        Assert.Contains("вече съществува", controller.ModelState["Name"]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Create_Post_NoUser_ShouldReturnUnauthorized()
    {
        var controllerWithoutUser = new AuthorsController(_authorServiceMock.Object);
        controllerWithoutUser.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() 
        };

        var model = new CreateAuthorViewModel { Name = "Test Author" };

        var result = await controllerWithoutUser.Create(model);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnViewWithModel_WhenAuthorExists()
    {
        var authorId = Guid.NewGuid();
        var author = new EditAuthorViewModel
        {
            Id = authorId,
            Name = "Test Author"
        };

        _authorServiceMock.Setup(s => s.GetByIdAsync(authorId))
            .ReturnsAsync(author);

        var result = await _controller.Edit(authorId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<EditAuthorViewModel>(viewResult.Model);

        Assert.Equal(authorId, model.Id);
        Assert.Equal("Test Author", model.Name);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnNotFound_WhenAuthorDoesNotExist()
    {
        var authorId = Guid.NewGuid();

        _authorServiceMock.Setup(s => s.GetByIdAsync(authorId))
            .ReturnsAsync((EditAuthorViewModel)null);

        var result = await _controller.Edit(authorId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ShouldUpdateAuthorAndRedirect_WhenModelIsValid()
    {
        var model = new EditAuthorViewModel
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name"
        };

        var result = await _controller.Edit(model);

        _authorServiceMock.Verify(s => s.UpdateAsync(model), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", redirectResult.ActionName);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnView_WhenModelIsInvalid()
    {
        var model = new EditAuthorViewModel
        {
            Id = Guid.NewGuid(),
            Name = "" 
        };

        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.Edit(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsAssignableFrom<EditAuthorViewModel>(viewResult.Model);
        Assert.Equal(model, returnedModel);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnView_WhenAuthorExists()
    {
        var authorId = Guid.NewGuid();
        var author = new EditAuthorViewModel
        {
            Id = authorId,
            Name = "Author to delete"
        };

        _authorServiceMock
            .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new EditAuthorViewModel
            {
                Id = authorId, 
                Name = "Author to delete"
            });

        var result = await _controller.Delete(authorId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<DeleteAuthorViewModel>(viewResult.Model);

        Assert.Equal(authorId, model.Id);
        Assert.Equal("Author to delete", model.Name);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnNotFound_WhenAuthorDoesNotExist()
    {
        var authorId = Guid.NewGuid();

        _authorServiceMock.Setup(s => s.GetByIdAsync(authorId))
            .ReturnsAsync((EditAuthorViewModel)null);

        var result = await _controller.Delete(authorId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldDeleteAuthorAndRedirect_WhenAuthorExists()
    {
        var authorId = Guid.NewGuid();
        var author = new EditAuthorViewModel
        {
            Id = authorId,
            Name = "Author to delete"
        };

        _authorServiceMock.Setup(s => s.GetByIdAsync(authorId))
            .ReturnsAsync(author);

        var result = await _controller.DeleteConfirmed(authorId);

        _authorServiceMock.Verify(s => s.DeleteAsync(authorId), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", redirectResult.ActionName);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldReturnNotFound_WhenAuthorDoesNotExist()
    {
        var authorId = Guid.NewGuid();

        _authorServiceMock.Setup(s => s.GetByIdAsync(authorId))
            .ReturnsAsync((EditAuthorViewModel)null);

        var result = await _controller.DeleteConfirmed(authorId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddBook_Get_ShouldReturnViewWithModel()
    {
        var authorId = Guid.NewGuid();
        var model = new AddBookToAuthorViewModel
        {
            AuthorId = authorId,
            Books = new List<BookDropdownViewModel>
            {
                new BookDropdownViewModel { Id = Guid.NewGuid(), Title = "Book 1" }
            }
        };

        _authorServiceMock.Setup(s => s.GetAddBookModelAsync(authorId))
            .ReturnsAsync(model);

        var result = await _controller.AddBook(authorId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsAssignableFrom<AddBookToAuthorViewModel>(viewResult.Model);
        Assert.Equal(authorId, returnedModel.AuthorId);
        Assert.Single(returnedModel.Books);
    }

    [Fact]
    public async Task AddBook_Post_ShouldReturnView_WhenModelStateIsInvalid()
    {
        var authorId = Guid.NewGuid();

        var model = new AddBookToAuthorViewModel
        {
            AuthorId = authorId,
            Books = new List<BookDropdownViewModel>()
        };

        _controller.ModelState.AddModelError("BookId", "Required");

        _authorServiceMock.Setup(s => s.GetAddBookModelAsync(authorId))
            .ReturnsAsync(new AddBookToAuthorViewModel
            {
                AuthorId = authorId,
                Books = new List<BookDropdownViewModel>
                {
                    new BookDropdownViewModel { Id = Guid.NewGuid(), Title = "Book A" }
                }
            });

        var result = await _controller.AddBook(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsAssignableFrom<AddBookToAuthorViewModel>(viewResult.Model);

        Assert.Equal(authorId, returnedModel.AuthorId);
        Assert.Single(returnedModel.Books);
    }
    [Fact]
    public async Task AddBook_Post_ShouldAddBookAndRedirect_WhenModelStateIsValid()
    {
        var authorId = Guid.NewGuid();

        var model = new AddBookToAuthorViewModel
        {
            AuthorId = authorId,
            SelectedBookId = Guid.NewGuid(),
            Books = new List<BookDropdownViewModel>()
        };

        var result = await _controller.AddBook(model);

        _authorServiceMock.Verify(s => s.AddBookToAuthorAsync(model), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", redirectResult.ActionName);
    }

}