using BookManager.Data;
using BookManager.Data.Models;
using BookManager.Services;
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


}