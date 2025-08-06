using BookManager.Services.Core.Contracts;
using BookManager.ViewModels.Publisher;
using BookManager.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookManager.Tests.Controllers
{
    public class PublishersControllerTests
    {
        private readonly Mock<IPublisherService> _publisherServiceMock;
        private readonly PublishersController _controller;

        public PublishersControllerTests()
        {
            _publisherServiceMock = new Mock<IPublisherService>();
            _controller = new PublishersController(_publisherServiceMock.Object);
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithPublishers()
        {
            var publishers = new List<PublisherDropdownViewModel>
            {
                new PublisherDropdownViewModel { Id = Guid.NewGuid(), Name = "Test Publisher" }
            };

            _publisherServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(publishers);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<PublisherDropdownViewModel>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public void Create_Get_ShouldReturnView()
        {
            var result = _controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<CreatePublisherViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task Create_Post_ShouldRedirect_WhenValidAndNotExists()
        {
            var model = new CreatePublisherViewModel { Name = "New Publisher" };
            _publisherServiceMock.Setup(s => s.ExistsByNameAsync(model.Name)).ReturnsAsync(false);

            var result = await _controller.Create(model);

            _publisherServiceMock.Verify(s => s.CreateAsync(model), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Create_Post_ShouldReturnViewWithError_WhenExists()
        {
            var model = new CreatePublisherViewModel { Name = "Existing Publisher" };
            _publisherServiceMock.Setup(s => s.ExistsByNameAsync(model.Name)).ReturnsAsync(true);

            var result = await _controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_Post_ShouldReturnView_WhenModelStateIsInvalid()
        {
            var mockService = new Mock<IPublisherService>();
            var controller = new PublishersController(mockService.Object);
            controller.ModelState.AddModelError("Name", "Required");

            var model = new CreatePublisherViewModel();

            var result = await controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_ShouldReturnView_WhenPublisherExists()
        {
            var id = Guid.NewGuid();
            var publisher = new PublisherViewModel { Id = id, Name = "Test" };
            _publisherServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(publisher);

            var result = await _controller.Edit(id);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PublisherViewModel>(viewResult.Model);
            Assert.Equal(id, model.Id);
        }

        [Fact]
        public async Task Edit_Get_ShouldReturnNotFound_WhenPublisherDoesNotExist()
        {
            var id = Guid.NewGuid();
            _publisherServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((PublisherViewModel)null);

            var result = await _controller.Edit(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ShouldRedirect_WhenModelValid()
        {
            var model = new PublisherViewModel { Id = Guid.NewGuid(), Name = "Updated Publisher" };

            var result = await _controller.Edit(model);

            _publisherServiceMock.Verify(s => s.UpdateAsync(model), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ShouldReturnView_WhenModelStateIsInvalid()
        {
            var mockService = new Mock<IPublisherService>();
            var controller = new PublishersController(mockService.Object);
            controller.ModelState.AddModelError("Name", "Required");

            var model = new PublisherViewModel
            {
                Id = Guid.NewGuid(),
                Name = ""
            };

            var result = await controller.Edit(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }


        [Fact]
        public async Task Delete_Get_ShouldReturnView_WhenPublisherExists()
        {
            var id = Guid.NewGuid();
            var publisher = new PublisherViewModel { Id = id, Name = "To Delete" };
            _publisherServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(publisher);

            var result = await _controller.Delete(id);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PublisherViewModel>(viewResult.Model);
            Assert.Equal(id, model.Id);
        }

        [Fact]
        public async Task Delete_Get_ShouldReturnNotFound_WhenPublisherIsNull()
        {
            var mockService = new Mock<IPublisherService>();
            mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((PublisherViewModel?)null);

            var controller = new PublishersController(mockService.Object);
            var nonExistentId = Guid.NewGuid();

            var result = await controller.Delete(nonExistentId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldRedirectAfterDelete()
        {
            var id = Guid.NewGuid();

            var result = await _controller.DeleteConfirmed(id);

            _publisherServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}