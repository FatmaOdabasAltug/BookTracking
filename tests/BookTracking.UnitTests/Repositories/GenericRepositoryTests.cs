using BookTracking.Domain.Entities;
using BookTracking.Infrastructure.Data;
using BookTracking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace BookTracking.UnitTests.Repositories;

public class GenericRepositoryTests
{
    private DbContextOptions<BookTrackingDbContext> GetOptions()
    {
        return new DbContextOptionsBuilder<BookTrackingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_ShouldPerformSoftDelete()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new GenericRepository<Book>(context);
        var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", IsActive = true, 
            Isbn = "1234567890", PublishDate = DateTime.UtcNow, Authors = new List<Author> { new Author { Id = Guid.NewGuid(), Name = "Author 1" } } };
        await repo.AddAsync(book);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(book);
        await context.SaveChangesAsync();

        var result = await context.Books.FindAsync(book.Id);
        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ShouldThrowConcurrencyException()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new GenericRepository<Book>(context);
        var nonExistentBook = new Book { Id = Guid.NewGuid(), Title = "Non-existent Book" ,Description="N/A", Isbn="N/A", PublishDate=DateTime.UtcNow, Authors=new List<Author>()};

        var act = async () => 
        {
            await repo.DeleteAsync(nonExistentBook);
            await context.SaveChangesAsync();
        };

        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }


    [Fact]
    public async Task AddAsync_And_GetByIdAsync_ShouldWorkInHarmony()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new GenericRepository<Book>(context);
        var book = new Book { Id = Guid.NewGuid(), Title = "Test Book 2", IsActive = true, 
            Isbn = "1234569990", PublishDate = DateTime.UtcNow, Authors = new List<Author> { new Author { Id = Guid.NewGuid(), Name = "Author 2" } } };

        await repo.AddAsync(book);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(book.Id);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Book 2");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingEntity()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new GenericRepository<Author>(context);
        var author = new Author { Id = Guid.NewGuid(), Name = "Old Name" };
        await repo.AddAsync(author);
        await context.SaveChangesAsync(); 

        author.Name = "New Name";
        await repo.UpdateAsync(author);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(author.Id);
        result!.Name.Should().Be("New Name");
        result.UpdatedAt.Should().NotBeNull();
    }
}