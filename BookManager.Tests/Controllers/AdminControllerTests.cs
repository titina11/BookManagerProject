using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Admin;
using BookManager.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class AdminControllerTests
{
    private readonly Mock<IAdminService> _adminServiceMock;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _adminServiceMock = new Mock<IAdminService>();
        _controller = new AdminController(_adminServiceMock.Object);
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithUserList()
    {
        var users = new List<UserWithRoleViewModel>
        {
            new UserWithRoleViewModel { Id = "1", UserName = "user1", Email = "user1@example.com", IsAdmin = true },
            new UserWithRoleViewModel { Id = "2", UserName = "user2", Email = "user2@example.com", IsAdmin = false }
        };

        _adminServiceMock
            .Setup(s => s.GetAllUsersAsync())
            .ReturnsAsync(users);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<UserWithRoleViewModel>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task RoleEdit_ShouldReturnViewWithUserList()
    {
        var users = new List<UserWithRoleViewModel>
        {
            new UserWithRoleViewModel { Id = "1", UserName = "user1", Email = "user1@example.com", IsAdmin = true }
        };

        _adminServiceMock
            .Setup(s => s.GetAllUsersAsync())
            .ReturnsAsync(users);

        var result = await _controller.RoleEdit();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("RoleEdit", viewResult.ViewName);
        var model = Assert.IsAssignableFrom<List<UserWithRoleViewModel>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task ToggleAdminRole_ShouldCallServiceAndRedirect()
    {
        var userId = "test-user-id";

        _adminServiceMock
            .Setup(s => s.ToggleAdminRoleAsync(userId))
            .ReturnsAsync(true) 
            .Verifiable();

        var result = await _controller.ToggleAdminRole(userId);

        _adminServiceMock.Verify(s => s.ToggleAdminRoleAsync(userId), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("RoleEdit", redirectResult.ActionName);
    }

    [Fact]
    public async Task ToggleAdminRole_ShouldHandleExceptionAndRedirect()
    {
        var invalidUserId = "invalid-user-id";

        _adminServiceMock
            .Setup(s => s.ToggleAdminRoleAsync(invalidUserId))
            .ThrowsAsync(new ArgumentException("User not found"));

        var tempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
        );
        _controller.TempData = tempData;

        var result = await _controller.ToggleAdminRole(invalidUserId);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("RoleEdit", redirectResult.ActionName);

        Assert.Equal("Грешка при промяна на ролята: User not found", _controller.TempData["Error"]);
    }

    [Fact]
    public async Task ToggleAdminRole_ShouldSetTempDataError_WhenRoleChangeFails()
    {
        var userId = "test-user-id";
        var adminServiceMock = new Mock<IAdminService>();
        adminServiceMock.Setup(s => s.ToggleAdminRoleAsync(userId)).ReturnsAsync(false);

        var controller = new AdminController(adminServiceMock.Object);

        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        var result = await controller.ToggleAdminRole(userId);

        Assert.True(controller.TempData.ContainsKey("Error"));
        Assert.Equal("Неуспешна промяна на ролята. Потребителят не е намерен или е невалиден.", controller.TempData["Error"]);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdminController.RoleEdit), redirect.ActionName);
    }


    [Fact]
    public async Task Index_ShouldReturnViewWithUsers()
    {
        var users = new List<UserWithRoleViewModel>
        {
            new UserWithRoleViewModel { Id = "1", UserName = "admin", Email = "admin@abv.bg", IsAdmin = true },
            new UserWithRoleViewModel { Id = "2", UserName = "user", Email = "user@abv.bg", IsAdmin = false }
        };

        _adminServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<UserWithRoleViewModel>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task RoleEdit_ShouldReturnViewWithUsers()
    {
        var users = new List<UserWithRoleViewModel>
        {
            new UserWithRoleViewModel { Id = "1", UserName = "admin", Email = "admin@abv.bg", IsAdmin = true }
        };

        _adminServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

        var result = await _controller.RoleEdit();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("RoleEdit", viewResult.ViewName);
        var model = Assert.IsAssignableFrom<List<UserWithRoleViewModel>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task RoleEdit_ShouldHandleException_AndReturnEmptyList()
    {
        _adminServiceMock.Setup(s => s.GetAllUsersAsync())
            .ThrowsAsync(new Exception("Database error"));

        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        _controller.TempData = tempData;

        var result = await _controller.RoleEdit();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("RoleEdit", viewResult.ViewName);
        Assert.NotNull(viewResult.Model);
        var model = Assert.IsAssignableFrom<List<UserWithRoleViewModel>>(viewResult.Model);
        Assert.Empty(model);
    }
}
