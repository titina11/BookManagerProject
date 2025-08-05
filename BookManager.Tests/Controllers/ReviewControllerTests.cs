using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Review;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace BookManager.Tests.Controllers;

public class ReviewControllerTests
{
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly ReviewController _controller;

    public ReviewControllerTests()
    {
        _reviewServiceMock = new Mock<IReviewService>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Role, "User")
        }, "mock"));

        _controller = new ReviewController(_reviewServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            }
        };
    }

    [Fact]
    public async Task AllForBook_ShouldReturnViewWithReviews()
    {
        var bookId = Guid.NewGuid();
        var reviews = new List<ReviewViewModel>
        {
            new ReviewViewModel { Id = Guid.NewGuid(), Content = "Review 1", Rating = 4 },
            new ReviewViewModel { Id = Guid.NewGuid(), Content = "Review 2", Rating = 5 }
        };

        _reviewServiceMock
            .Setup(s => s.GetReviewsForBookAsync(bookId))
            .ReturnsAsync(reviews);

        var result = await _controller.AllForBook(bookId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ReviewViewModel>>(viewResult.Model);

        Assert.Equal(2, model.Count);
        Assert.Equal("Review 1", model[0].Content);
        Assert.Equal(4, model[0].Rating);

        Assert.Equal(bookId, _controller.ViewBag.BookId);
    }

    [Fact]
    public async Task AllForBook_ShouldReturnEmptyList_WhenNoReviews()
    {
        var bookId = Guid.NewGuid();

        _reviewServiceMock
            .Setup(s => s.GetReviewsForBookAsync(bookId))
            .ReturnsAsync(new List<ReviewViewModel>());

        var result = await _controller.AllForBook(bookId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ReviewViewModel>>(viewResult.Model);

        Assert.Empty(model);
        Assert.Equal(bookId, _controller.ViewBag.BookId);
    }

    [Fact]
    public async Task AllForBook_ShouldThrow_WhenServiceFails()
    {
        var bookId = Guid.NewGuid();

        _reviewServiceMock
            .Setup(s => s.GetReviewsForBookAsync(bookId))
            .ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _controller.AllForBook(bookId));
    }

    [Fact]
    public async Task Create_Get_ShouldReturnViewWithModel()
    {
        var bookId = Guid.NewGuid();
        var model = new CreateReviewViewModel { BookId = bookId };

        _reviewServiceMock
            .Setup(s => s.GetCreateModelAsync(bookId))
            .ReturnsAsync(model);

        var result = await _controller.Create(bookId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsAssignableFrom<CreateReviewViewModel>(viewResult.Model);
        Assert.Equal(bookId, returnedModel.BookId);
    }

    [Fact]
    public async Task Create_Post_ShouldCallServiceAndRedirect_WhenModelIsValid()
    {
        var model = new CreateReviewViewModel
        {
            BookId = Guid.NewGuid(),
            Content = "Test review",
            Rating = 4
        };

        _reviewServiceMock
            .Setup(s => s.CreateAsync(model, "test-user-id"))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _controller.Create(model);

        _reviewServiceMock.Verify(s => s.CreateAsync(model, "test-user-id"), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AllForBook", redirectResult.ActionName);
        Assert.Equal("Review", redirectResult.ControllerName);
        Assert.Equal(model.BookId, redirectResult.RouteValues["bookId"]);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnView_WhenModelStateIsInvalid()
    {
        var model = new CreateReviewViewModel
        {
            BookId = Guid.NewGuid()
        };

        _controller.ModelState.AddModelError("Content", "Required");

        var result = await _controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<CreateReviewViewModel>(viewResult.Model);
        Assert.Equal(model.BookId, returnedModel.BookId);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnView_WhenReviewExists_AndUserIsOwner()
    {
        var reviewId = Guid.NewGuid();

        var reviewModel = new EditReviewViewModel
        {
            Id = reviewId,
            UserId = "test-user-id",
            BookId = Guid.NewGuid(),
            Content = "Test content",
            Rating = 5
        };

        _reviewServiceMock
            .Setup(s => s.GetEditModelAsync(reviewId))
            .ReturnsAsync(reviewModel);

        var result = await _controller.Edit(reviewId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditReviewViewModel>(viewResult.Model);

        Assert.Equal(reviewId, model.Id);
        Assert.Equal("test-user-id", model.UserId);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnNotFound_WhenReviewDoesNotExist()
    {
        var reviewId = Guid.NewGuid();

        _reviewServiceMock
            .Setup(s => s.GetEditModelAsync(reviewId))
            .ReturnsAsync((EditReviewViewModel?)null);

        var result = await _controller.Edit(reviewId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ShouldReturnForbid_WhenUserIsNotOwnerAndNotAdmin()
    {
        var reviewId = Guid.NewGuid();

        var reviewModel = new EditReviewViewModel
        {
            Id = reviewId,
            UserId = "other-user-id",
            BookId = Guid.NewGuid(),
            Content = "Test content",
            Rating = 4
        };

        _reviewServiceMock
            .Setup(s => s.GetEditModelAsync(reviewId))
            .ReturnsAsync(reviewModel);

        var result = await _controller.Edit(reviewId);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ShouldCallServiceAndRedirect_WhenModelIsValid_AndUserIsOwner()
    {
        var reviewId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

        var model = new EditReviewViewModel
        {
            Id = reviewId,
            UserId = "test-user-id",
            BookId = bookId,
            Content = "Updated review",
            Rating = 5
        };

        var result = await _controller.Edit(reviewId, model);

        _reviewServiceMock.Verify(s =>
            s.EditReviewAsync(reviewId, model, "test-user-id", false), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AllForBook", redirectResult.ActionName);
        Assert.Equal("Review", redirectResult.ControllerName);
        Assert.Equal(bookId, redirectResult.RouteValues["bookId"]);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnView_WhenModelStateIsInvalid()
    {
        var reviewId = Guid.NewGuid();

        var model = new EditReviewViewModel
        {
            Id = reviewId,
            BookId = Guid.NewGuid(),
            Content = "",
            Rating = 0
        };

        _controller.ModelState.AddModelError("Content", "Required");

        var result = await _controller.Edit(reviewId, model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<EditReviewViewModel>(viewResult.Model);
        Assert.Equal(model.Id, returnedModel.Id);

        _reviewServiceMock.Verify(
            s => s.EditReviewAsync(It.IsAny<Guid>(), It.IsAny<EditReviewViewModel>(), It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task Edit_Post_ShouldCallService_WithIsAdminTrue_WhenUserIsAdmin()
    {
        var reviewId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

        var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.NameIdentifier, "admin-id"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = adminUser }
        };

        var model = new EditReviewViewModel
        {
            Id = reviewId,
            UserId = "other-user-id",
            BookId = bookId,
            Content = "Admin edit",
            Rating = 4
        };

        var result = await _controller.Edit(reviewId, model);

        _reviewServiceMock.Verify(s =>
            s.EditReviewAsync(reviewId, model, "admin-id", true), Times.Once);

        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnView_WhenReviewExistsAndUserIsOwner()
    {
        var reviewId = Guid.NewGuid();

        var model = new EditReviewViewModel
        {
            Id = reviewId,
            BookId = Guid.NewGuid(),
            UserId = "test-user-id",
            Content = "Review content",
            Rating = 5
        };

        _reviewServiceMock
            .Setup(s => s.GetEditModelAsync(reviewId))
            .ReturnsAsync(model);

        var result = await _controller.Delete(reviewId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<EditReviewViewModel>(viewResult.Model);
        Assert.Equal(model.Id, returnedModel.Id);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnNotFound_WhenReviewDoesNotExist()
    {
        var reviewId = Guid.NewGuid();

        _reviewServiceMock
            .Setup(s => s.GetEditModelAsync(reviewId))
            .ReturnsAsync((EditReviewViewModel?)null);

        var result = await _controller.Delete(reviewId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ShouldReturnForbid_WhenUserIsNotOwnerOrAdmin()
    {
        var reviewId = Guid.NewGuid();

        var model = new EditReviewViewModel
        {
            Id = reviewId,
            BookId = Guid.NewGuid(),
            UserId = "other-user-id",
            Content = "Review content",
            Rating = 5
        };

        _reviewServiceMock
            .Setup(s => s.GetEditModelAsync(reviewId))
            .ReturnsAsync(model);

        var nonOwnerUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = nonOwnerUser }
        };

        var result = await _controller.Delete(reviewId);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task ConfirmDelete_ShouldDeleteReviewAndRedirect_WhenUserIsOwner()
    {
        var reviewId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var userId = "test-user-id";

        var model = new EditReviewViewModel
        {
            Id = reviewId,
            BookId = bookId,
            UserId = userId,
            Content = "Review content",
            Rating = 5
        };

        _reviewServiceMock
            .Setup(s => s.GetEditModelAsync(reviewId))
            .ReturnsAsync(model);

        _reviewServiceMock
            .Setup(s => s.DeleteReviewAsync(reviewId, userId, false))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _controller.ConfirmDelete(reviewId);

        _reviewServiceMock.Verify();
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AllForBook", redirect.ActionName);
        Assert.Equal(bookId, redirect.RouteValues["bookId"]);
    }

    [Fact]
    public async Task ConfirmDelete_ShouldReturnNotFound_WhenReviewDoesNotExist()
    {
        var reviewId = Guid.NewGuid();

        _reviewServiceMock
            .Setup(s => s.GetEditModelAsync(reviewId))
            .ReturnsAsync((EditReviewViewModel?)null);

        var result = await _controller.ConfirmDelete(reviewId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ConfirmDelete_ShouldDeleteReview_WhenUserIsAdmin()
    {
        var reviewId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

        var model = new EditReviewViewModel
        {
            Id = reviewId,
            BookId = bookId,
            UserId = "other-user-id"
        };

        _reviewServiceMock
            .Setup(s => s.GetEditModelAsync(reviewId))
            .ReturnsAsync(model);

        _reviewServiceMock
            .Setup(s => s.DeleteReviewAsync(reviewId, "test-user-id", true))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = adminUser }
        };

        var result = await _controller.ConfirmDelete(reviewId);

        _reviewServiceMock.Verify();
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AllForBook", redirect.ActionName);
        Assert.Equal(bookId, redirect.RouteValues["bookId"]);
    }
}
