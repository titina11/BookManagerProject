using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services;
using BookManager.Services.Core;
using BookManager.ViewModels.Publisher;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
public class PublisherServiceTests
{
    private BookManagerDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BookManagerDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPublishers()
    {
        var context = GetDbContext();
        context.Publishers.AddRange(
            new Publisher { Id = Guid.NewGuid(), Name = "Publisher One" },
            new Publisher { Id = Guid.NewGuid(), Name = "Publisher Two" }
        );
        await context.SaveChangesAsync();

        var service = new PublisherService(context);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Publisher One");
        Assert.Contains(result, p => p.Name == "Publisher Two");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoPublishersExist()
    {
        var context = GetDbContext();
        var service = new PublisherService(context);

        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldReturnTrue_WhenPublisherExists()
    {
        var context = GetDbContext();
        context.Publishers.Add(new Publisher { Id = Guid.NewGuid(), Name = "Orbit" });
        await context.SaveChangesAsync();

        var service = new PublisherService(context);

        var result = await service.ExistsByNameAsync("Orbit");

        Assert.True(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectPublisher()
    {
        var context =GetDbContext();
        var publisher = new Publisher { Id = Guid.NewGuid(), Name = "Test Publisher" };
        context.Publishers.Add(publisher);
        await context.SaveChangesAsync();

        var service = new PublisherService(context);

        var result = await service.GetByIdAsync(publisher.Id);

        Assert.NotNull(result);
        Assert.Equal(publisher.Name, result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenPublisherNotFound()
    {
        var dbContext = GetDbContext();
        var service = new PublisherService(dbContext);

        var result = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPublishersAsDropdownViewModel()
    {
        var context = GetDbContext();

        context.Publishers.AddRange(
            new Publisher { Id = Guid.NewGuid(), Name = "Издателство А" },
            new Publisher { Id = Guid.NewGuid(), Name = "Издателство Б" });

        await context.SaveChangesAsync();

        var service = new PublisherService(context);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.IsType<PublisherDropdownViewModel>(p));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExistsByNameAsync_ShouldReturnFalse_WhenNameIsNullOrWhitespace(string input)
    {
        var context = GetDbContext();
        var service = new PublisherService(context);

        var result = await service.ExistsByNameAsync(input);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddPublisher()
    {
        var context = GetDbContext();
        var service = new PublisherService(context);

        var model = new CreatePublisherViewModel { Name = "New Publisher" };
        await service.CreateAsync(model);

        var result = await context.Publishers.FirstOrDefaultAsync(p => p.Name == "New Publisher");
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldChangePublisherName()
    {
        var context = GetDbContext();
        var publisher = new Publisher { Id = Guid.NewGuid(), Name = "Old Name" };
        context.Publishers.Add(publisher);
        await context.SaveChangesAsync();

        var service = new PublisherService(context);
        var updatedModel = new PublisherViewModel { Id = publisher.Id, Name = "New Name" };
        await service.UpdateAsync(updatedModel);

        var updated = await context.Publishers.FindAsync(publisher.Id);
        Assert.Equal("New Name", updated!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotThrow_WhenPublisherNotFound()
    {
        var dbContext = GetDbContext();
        var service = new PublisherService(dbContext);

        var model = new PublisherViewModel
        {
            Id = Guid.NewGuid(),
            Name = "Does Not Exist"
        };

        var exception = await Record.ExceptionAsync(() => service.UpdateAsync(model));
        Assert.Null(exception);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemovePublisher()
    {
        var context = GetDbContext();
        var publisher = new Publisher { Id = Guid.NewGuid(), Name = "To Be Deleted" };
        context.Publishers.Add(publisher);
        await context.SaveChangesAsync();

        var service = new PublisherService(context);
        await service.DeleteAsync(publisher.Id);

        var deleted = await context.Publishers.FindAsync(publisher.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotThrow_WhenPublisherNotFound()
    {
        var dbContext = GetDbContext();
        var service = new PublisherService(dbContext);

        var nonExistentId = Guid.NewGuid();

        var exception = await Record.ExceptionAsync(() => service.DeleteAsync(nonExistentId));
        Assert.Null(exception);
    }

}