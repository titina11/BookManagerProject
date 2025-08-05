using BookManager.Controllers;
using BookManager.Data.Models;
using BookManager.ViewModels.UserBooks;
using BookManager.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookManager.Tests.Controllers;

public class UserBooksControllerTests
{
    private BookManagerDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BookManagerDbContext(options);
    }

    private ClaimsPrincipal GetTestUser(string userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task MyBooks_ShouldReturnUserBooks()
    {
        var context = GetDbContext();
        var controller = new UserBooksController(context);

        var userId = "user123";
        var otherUserId = "someone-else";

        var book1 = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Book 1",
            CreatedByUserId = userId,
            Description = "Desc"
        };

        var book2 = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Book 2",
            CreatedByUserId = otherUserId,
            Description = "Other desc"
        };

        context.Books.AddRange(book1, book2);

        var userBook1 = new UserBook
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = book1.Id,
            StartDate = DateTime.Today.AddDays(-2),
            EndDate = DateTime.Today,
            Rating = 4
        };

        var userBook2 = new UserBook
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            BookId = book2.Id,
            StartDate = DateTime.Today.AddDays(-3),
            EndDate = DateTime.Today.AddDays(-1),
            Rating = 5
        };

        context.UserBooks.AddRange(userBook1, userBook2);
        await context.SaveChangesAsync();

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await controller.MyBooks();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<UserBookListViewModel>>(viewResult.Model);

        Assert.Single(model);
        Assert.Equal(book1.Title, model[0].Title);
        Assert.Equal(userBook1.Rating, model[0].Rating);
    }
    [Fact]
    public async Task Add_Get_ShouldReturnView_WhenBookExists()
    {
        var context = GetDbContext();
        var bookId = Guid.NewGuid();

        context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            CreatedByUserId = "test-user-id",
            Description = "Test book description"
        });

        await context.SaveChangesAsync();

        var controller = new UserBooksController(context);

        var result = await controller.Add(bookId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserBookListViewModel>(viewResult.Model);

        Assert.Equal(bookId, model.BookId);
        Assert.Equal(DateTime.Today, model.StartDate);
        Assert.Equal(DateTime.Today.AddDays(1), model.EndDate);
        Assert.Equal(3, model.Rating);
    }

    [Fact]
    public async Task Add_Get_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        var context = GetDbContext();
        var controller = new UserBooksController(context);
        var invalidBookId = Guid.NewGuid();

        var result = await controller.Add(invalidBookId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Add_Post_ShouldReturnViewWithBookTitle_WhenModelStateIsInvalid()
    {
        var options = new DbContextOptionsBuilder<BookManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new BookManagerDbContext(options);

        var bookId = Guid.NewGuid();
        context.Books.Add(new Book { Id = bookId, Title = "Test Book", Description = "Test Description", CreatedByUserId = "user-123",
        });
        await context.SaveChangesAsync();

        var controller = new UserBooksController(context);

        var invalidModel = new UserBookListViewModel
        {
            BookId = bookId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(-1), 
            Rating = 5
        };

        controller.ModelState.AddModelError("ReadEndDate", "End date must be after start date.");

        var result = await controller.Add(invalidModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<UserBookListViewModel>(viewResult.Model);

        Assert.Equal(invalidModel.BookId, returnedModel.BookId);
        Assert.Equal("Test Book", controller.ViewBag.BookTitle);
    }


    [Fact]
    public async Task Add_Post_ShouldAddUserBookAndRedirect_WhenModelIsValid()
    {
        var context = GetDbContext();
        var controller = new UserBooksController(context);

        var bookId = Guid.NewGuid();

        context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            CreatedByUserId = "user1",
            Description = "Some description"
        });

        await context.SaveChangesAsync();

        var model = new UserBookListViewModel
        {
            BookId = bookId,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Rating = 4
        };

        var userId = "test-user-id";
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await controller.Add(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("MyBooks", redirect.ActionName);

        var userBook = await context.UserBooks.FirstOrDefaultAsync();
        Assert.NotNull(userBook);
        Assert.Equal(userId, userBook!.UserId);
        Assert.Equal(bookId, userBook.BookId);
        Assert.Equal(model.StartDate, userBook.StartDate);
        Assert.Equal(model.EndDate, userBook.EndDate);
        Assert.Equal(model.Rating, userBook.Rating);
    }

    [Fact]
    public async Task Add_Post_ShouldReturnView_WhenModelIsInvalid()
    {
        var context = GetDbContext();
        var controller = new UserBooksController(context);

        var bookId = Guid.NewGuid();

        context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Invalid Book",
            CreatedByUserId = "user1",
            Description = "Dummy"
        });

        await context.SaveChangesAsync();

        var model = new UserBookListViewModel
        {
            BookId = bookId,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Rating = 5
        };

        controller.ModelState.AddModelError("SomeField", "Required");

        var result = await controller.Add(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<UserBookListViewModel>(viewResult.Model);

        Assert.Equal(bookId, returnedModel.BookId);
        Assert.Equal("Invalid Book", controller.ViewBag.BookTitle);
    }

    [Fact]
    public async Task Delete_ShouldReturnView_WhenUserBookExists()
    {
        var context = GetDbContext();
        var controller = new UserBooksController(context);

        var userId = "user123";
        var bookId = Guid.NewGuid();
        var userBookId = Guid.NewGuid();

        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            CreatedByUserId = userId,
            Description = "A description"
        };

        var userBook = new UserBook
        {
            Id = userBookId,
            UserId = userId,
            BookId = bookId,
            StartDate = DateTime.Today.AddDays(-5),
            EndDate = DateTime.Today,
            Rating = 5
        };

        context.Books.Add(book);
        context.UserBooks.Add(userBook);
        await context.SaveChangesAsync();

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await controller.Delete(userBookId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ReadBookViewModel>(viewResult.Model);

        Assert.Equal(userBookId, model.Id);
        Assert.Equal("Test Book", model.BookTitle);
        Assert.Equal(5, model.Rating);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenUserBookDoesNotExist()
    {
        var context = GetDbContext();
        var controller = new UserBooksController(context);

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user123") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await controller.Delete(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldDeleteAndRedirect_WhenUserBookExists()
    {
        var context = GetDbContext();

        var userId = "user123";
        var bookId = Guid.NewGuid();
        var userBookId = Guid.NewGuid();

        context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Book",
            Description = "Test",
            CreatedByUserId = userId
        });

        context.UserBooks.Add(new UserBook
        {
            Id = userBookId,
            BookId = bookId,
            UserId = userId,
            StartDate = DateTime.Today.AddDays(-3),
            EndDate = DateTime.Today,
            Rating = 4
        });

        await context.SaveChangesAsync();

        var controller = new UserBooksController(context);
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) }
        };

        var result = await controller.DeleteConfirmed(userBookId);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("MyBooks", redirectResult.ActionName);

        var userBook = await context.UserBooks.FirstOrDefaultAsync(ub => ub.Id == userBookId);
        Assert.Null(userBook); 
    }

    [Fact]
    public async Task DeleteConfirmed_ShouldReturnNotFound_WhenUserBookDoesNotExist()
    {
        var context = GetDbContext();
        var controller = new UserBooksController(context);

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user123") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await controller.DeleteConfirmed(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

}