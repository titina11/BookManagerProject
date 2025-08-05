using BookManager.Data.Configuration;
using BookManager.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookManager.Tests.Services
{
    public class IdentitySeederTests
    {
        [Fact]
        public async Task SeedRolesAndAdminAsync_ShouldCreateRolesAndAdmin_WhenTheyDoNotExist()
        {
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null!, null!, null!, null!);

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(RoleManager<IdentityRole>)))
                               .Returns(roleManagerMock.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(UserManager<ApplicationUser>)))
                               .Returns(userManagerMock.Object);

            await IdentitySeeder.SeedRolesAndAdminAsync(serviceProviderMock.Object);

            roleManagerMock.Verify(r => r.CreateAsync(It.Is<IdentityRole>(role => role.Name == "Admin")), Times.Once);
            roleManagerMock.Verify(r => r.CreateAsync(It.Is<IdentityRole>(role => role.Name == "User")), Times.Once);
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), "AdminPassword123!"), Times.Once);
            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Admin"), Times.Once);
        }

        [Fact]
        public async Task SeedRolesAndAdminAsync_ShouldNotCreateAdmin_WhenUserAlreadyExists()
        {
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null!, null!, null!, null!);

            var existingAdmin = new ApplicationUser { Email = "admin@example.com" };

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(existingAdmin);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(RoleManager<IdentityRole>)))
                               .Returns(roleManagerMock.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(UserManager<ApplicationUser>)))
                               .Returns(userManagerMock.Object);

            await IdentitySeeder.SeedRolesAndAdminAsync(serviceProviderMock.Object);

            roleManagerMock.Verify(r => r.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SeedRolesAndAdminAsync_ShouldThrowException_WhenAdminCreationFails()
        {
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null!, null!, null!, null!);

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Creation failed" }));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(RoleManager<IdentityRole>)))
                               .Returns(roleManagerMock.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(UserManager<ApplicationUser>)))
                               .Returns(userManagerMock.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                IdentitySeeder.SeedRolesAndAdminAsync(serviceProviderMock.Object));

            Assert.Contains("Admin user creation failed", ex.Message);
        }
    }
}
