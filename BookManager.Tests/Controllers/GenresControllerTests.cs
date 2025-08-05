using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Genre;
using BookManager.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookManager.Tests.Controllers;

public class GenresControllerTests
{
    private readonly Mock<IGenreService> _genreServiceMock;
    private readonly GenresController _controller;

    public GenresControllerTests()
    {
        _genreServiceMock = new Mock<IGenreService>();
        _controller = new GenresController(_genreServiceMock.Object);
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithGenres()
    {
        var genres = new List<GenreViewModel>
        {
            new GenreViewModel { Id = Guid.NewGuid(), Name = "Fiction" },
            new GenreViewModel { Id = Guid.NewGuid(), Name = "History" }
        };

        _genreServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(genres);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<GenreViewModel>>(viewResult.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public void Create_Get_ShouldReturnView()
    {
        var result = _controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_ShouldRedirectToIndex_WhenModelIsValid()
    {
        var model = new GenreViewModel { Name = "Fantasy" };

        var result = await _controller.Create(model);

        _genreServiceMock.Verify(s => s.CreateAsync(model), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnView_WhenModelIsInvalid()
    {
        var model = new GenreViewModel { Name = "" };
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnView_WhenGenreExists()
    {
        var genre = new GenreViewModel { Id = Guid.NewGuid(), Name = "Thriller" };
        _genreServiceMock.Setup(s => s.GetByIdAsync(genre.Id)).ReturnsAsync(genre);

        var result = await _controller.Edit(genre.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GenreViewModel>(viewResult.Model);
        Assert.Equal("Thriller", model.Name);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnNotFound_WhenGenreDoesNotExist()
    {
        var id = Guid.NewGuid();
        _genreServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((GenreViewModel?)null);

        var result = await _controller.Edit(id);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ShouldRedirectToIndex_WhenModelIsValid()
    {
        var model = new GenreViewModel { Id = Guid.NewGuid(), Name = "Horror" };

        var result = await _controller.Edit(model);

        _genreServiceMock.Verify(s => s.UpdateAsync(model), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnView_WhenModelIsInvalid()
    {
        var model = new GenreViewModel { Name = "" };
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.Edit(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnView_WhenGenreExists()
    {
        var genre = new GenreViewModel { Id = Guid.NewGuid(), Name = "Drama" };
        _genreServiceMock.Setup(s => s.GetByIdAsync(genre.Id)).ReturnsAsync(genre);

        var result = await _controller.Delete(genre.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(genre, viewResult.Model);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnNotFound_WhenGenreDoesNotExist()
    {
        var id = Guid.NewGuid();
        _genreServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((GenreViewModel?)null);

        var result = await _controller.Delete(id);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldDeleteAndRedirect()
    {
        var id = Guid.NewGuid();

        var result = await _controller.DeleteConfirmed(id);

        _genreServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
}