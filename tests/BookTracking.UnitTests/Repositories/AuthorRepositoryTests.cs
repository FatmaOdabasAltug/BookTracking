using BookTracking.Domain.Entities;
using BookTracking.Infrastructure.Data;
using BookTracking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace BookTracking.UnitTests.Repositories;

public class AuthorRepositoryTests
{
    private DbContextOptions<BookTrackingDbContext> GetOptions()
    {
        return new DbContextOptionsBuilder<BookTrackingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;
    }

    [Fact]
    public async Task GetAuthorListAsync_ShouldReturnAuthors_ForGivenIds()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuthorRepository(context);
        
        var author1 = new Author { Id = Guid.NewGuid(), Name = "Author 1" };
        var author2 = new Author { Id = Guid.NewGuid(), Name = "Author 2" };
        var author3 = new Author { Id = Guid.NewGuid(), Name = "Author 3" };

        await repo.AddAsync(author1);
        await repo.AddAsync(author2);
        await repo.AddAsync(author3);
        await context.SaveChangesAsync();

        var ids = new List<Guid> { author1.Id, author3.Id };
        var result = await repo.GetAuthorListAsync(ids);

        result.Should().HaveCount(2);
        result.Select(a => a!.Name).Should().Contain("Author 1");
        result.Select(a => a!.Name).Should().Contain("Author 3");
        result.Select(a => a!.Name).Should().NotContain("Author 2");
    }

    [Fact]
    public async Task GetByIdWithBooksAsync_ShouldReturnAuthorWithBooks()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuthorRepository(context);
        
        var book = new Book { Id = Guid.NewGuid(), Title = "Book 1", Isbn="111", PublishDate=DateTime.UtcNow, IsActive=true, Authors = new List<Author>() };
        var author = new Author { 
            Id = Guid.NewGuid(), 
            Name = "Author 1", 
            Books = new List<Book> { book } 
        };

        await repo.AddAsync(author);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdWithBooksAsync(author.Id);

        result.Should().NotBeNull();
        result!.Books.Should().HaveCount(1);
        result.Books.First().Title.Should().Be("Book 1");
    }

    [Fact]
    public async Task GetByIdWithBooksAsync_ShouldReturnNull_WhenAuthorDoesNotExist()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuthorRepository(context);

        var result = await repo.GetByIdWithBooksAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAuthorListAsync_ShouldReturnEmpty_WhenNoIdsMatch()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuthorRepository(context);
        
        await repo.AddAsync(new Author { Id = Guid.NewGuid(), Name = "A" });
        await context.SaveChangesAsync();

        var result = await repo.GetAuthorListAsync(new List<Guid> { Guid.NewGuid() });

        result.Should().BeEmpty();
    }
}
