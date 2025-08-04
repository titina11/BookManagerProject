using BookManager.Data.Models;
using BookManager.Services.Core;
using BookManager.ViewModels.Admin;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookManager.Tests.Services.Core
{
    public class AdminServiceTests
    {
        private (BookManagerDbContext context, UserManager<ApplicationUser> userManager) GetTestEnvironment()
        {
            var services = new ServiceCollection();

            services.AddDbContext<BookManagerDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<BookManagerDbContext>()
                .AddDefaultTokenProviders();
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();

            var context = serviceProvider.GetRequiredService<BookManagerDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!roleManager.Roles.Any())
            {
                var role = new IdentityRole("Admin");
                roleManager.CreateAsync(role).Wait();
            }

            return (context, userManager);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnUserWithCorrectRole()
        {
            var (context, userManager) = GetTestEnvironment();

            var adminUser = new ApplicationUser
            {
                Id = "admin-id",
                UserName = "admin",
                Email = "admin@example.com"
            };

            var normalUser = new ApplicationUser
            {
                Id = "user-id",
                UserName = "user",
                Email = "user@example.com"
            };

            await userManager.CreateAsync(adminUser, "Test123!");
            await userManager.CreateAsync(normalUser, "Test123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");

            var service = new AdminService(userManager);

            var result = await service.GetAllUsersAsync();

            Assert.Equal(2, result.Count);
            var adminVm = result.FirstOrDefault(u => u.UserName == "admin");
            var userVm = result.FirstOrDefault(u => u.UserName == "user");

            Assert.NotNull(adminVm);
            Assert.True(adminVm.IsAdmin);

            Assert.NotNull(userVm);
            Assert.False(userVm.IsAdmin);
        }

        [Fact]
        public async Task ToggleAdminRoleAsync_ShouldAddOrRemoveAdminRole()
        {
            var user = new ApplicationUser { Id = "user1", UserName = "user1" };

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null
            );

            mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync(user);
            mockUserManager.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
            mockUserManager.Setup(m => m.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

            var service = new AdminService(mockUserManager.Object);

            var result = await service.ToggleAdminRoleAsync("user1");

            Assert.True(result);
            mockUserManager.Verify(m => m.AddToRoleAsync(user, "Admin"), Times.Once);
        }

        [Fact]
        public async Task ToggleAdminRoleAsync_ShouldRemoveRole_WhenAlreadyAdmin()
        {
            var user = new ApplicationUser { Id = "user2", UserName = "user2" };

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null
            );

            mockUserManager.Setup(m => m.FindByIdAsync("user2")).ReturnsAsync(user);
            mockUserManager.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);
            mockUserManager.Setup(m => m.RemoveFromRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

            var service = new AdminService(mockUserManager.Object);

            var result = await service.ToggleAdminRoleAsync("user2");

            Assert.True(result);
            mockUserManager.Verify(m => m.RemoveFromRoleAsync(user, "Admin"), Times.Once);
        }

        [Fact]
        public async Task ToggleAdminRoleAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null
            );

            mockUserManager.Setup(m => m.FindByIdAsync("missing")).ReturnsAsync((ApplicationUser?)null);

            var service = new AdminService(mockUserManager.Object);

            var result = await service.ToggleAdminRoleAsync("missing");

            Assert.False(result);
        }
    }
}
