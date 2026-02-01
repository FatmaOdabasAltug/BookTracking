using BookTracking.Domain.Entities;
using BookTracking.Infrastructure.Data;
using BookTracking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace BookTracking.UnitTests.Repositories;

public class BookRepositoryTests
{
    private DbContextOptions<BookTrackingDbContext> GetOptions()
    {
        return new DbContextOptionsBuilder<BookTrackingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;
    }

    [Fact]
    public async Task AnyByIsbnAsync_ShouldReturnTrue_WhenIsbnExists()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new BookRepository(context);
        var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", Isbn = "1234567890", IsActive=true, PublishDate = DateTime.UtcNow, Authors = new List<Author>() };
        
        await repo.AddAsync(book);
        await context.SaveChangesAsync();

        var result = await repo.AnyByIsbnAsync("1234567890");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyByIsbnAsync_ShouldReturnFalse_WhenIsbnDoesNotExist()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new BookRepository(context);
        
        var result = await repo.AnyByIsbnAsync("0000000000");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdWithAuthorsAsync_ShouldReturnBookWithAuthors()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new BookRepository(context);
        var author = new Author { Id = Guid.NewGuid(), Name = "Author 1" };
        var book = new Book { 
            Id = Guid.NewGuid(), 
            Title = "Test Book", 
            Isbn = "1234567890", 
            IsActive = true,
            PublishDate = DateTime.UtcNow, 
            Authors = new List<Author> { author } 
        };

        await repo.AddAsync(book);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdWithAuthorsAsync(book.Id);

        result.Should().NotBeNull();
        result!.Authors.Should().HaveCount(1);
        result.Authors.First().Name.Should().Be("Author 1");
    }

    [Fact]
    public async Task GetByIdWithAuthorsAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new BookRepository(context);

        var result = await repo.GetByIdWithAuthorsAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}
