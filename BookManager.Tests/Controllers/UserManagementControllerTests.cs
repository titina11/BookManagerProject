using BookManager.Data.Models;
using BookManager.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace BookManager.Tests.Controllers;

public class UserManagementControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly UserManagementController _controller;

    public UserManagementControllerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _controller = new UserManagementController(_userManagerMock.Object);
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithUserList()
    {
        var userList = new List<ApplicationUser>
        {
            new ApplicationUser { Id = "1", UserName = "user1" },
            new ApplicationUser { Id = "2", UserName = "user2" }
        };

        _userManagerMock.Setup(m => m.Users).Returns(userList.AsQueryable());
        _userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), "Admin"))
                        .ReturnsAsync(false);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<(ApplicationUser, bool)>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task ToggleAdminRole_ShouldAddRole_IfNotAdmin()
    {
        var user = new ApplicationUser { Id = "123", UserName = "testuser" };

        _userManagerMock.Setup(m => m.FindByIdAsync("123")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
        _userManagerMock.Setup(m => m.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

        var result = await _controller.ToggleAdminRole("123");

        Assert.IsType<RedirectToActionResult>(result);
        _userManagerMock.Verify(m => m.AddToRoleAsync(user, "Admin"), Times.Once);
    }

    [Fact]
    public async Task ToggleAdminRole_ShouldRemoveRole_IfAdmin()
    {
        var user = new ApplicationUser { Id = "456", UserName = "adminuser" };

        _userManagerMock.Setup(m => m.FindByIdAsync("456")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.RemoveFromRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

        var result = await _controller.ToggleAdminRole("456");

        Assert.IsType<RedirectToActionResult>(result);
        _userManagerMock.Verify(m => m.RemoveFromRoleAsync(user, "Admin"), Times.Once);
    }

    [Fact]
    public async Task ToggleAdminRole_ShouldReturnNotFound_IfUserDoesNotExist()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("999")).ReturnsAsync((ApplicationUser?)null);

        var result = await _controller.ToggleAdminRole("999");

        Assert.IsType<NotFoundResult>(result);
    }
}