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
        var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", IsActive = true };
        await repo.AddAsync(book);
        await repo.DeleteAsync(book.Id);

        var result = await context.Books.FindAsync(book.Id);
        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new GenericRepository<Book>(context);
        var nonExistentId = Guid.NewGuid();

        var act = async () => await repo.DeleteAsync(nonExistentId);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Book with ID {nonExistentId} not found.");
    }


    [Fact]
    public async Task AddAsync_And_GetByIdAsync_ShouldWorkInHarmony()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new GenericRepository<Book>(context);
        var book = new Book { Id = Guid.NewGuid(), Title = "Book 2" };

        await repo.AddAsync(book);

        var result = await repo.GetByIdAsync(book.Id);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Book 2");
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

        author.Name = "New Name";
        await repo.UpdateAsync(author);

        var result = await repo.GetByIdAsync(author.Id);
        result!.Name.Should().Be("New Name");
        result.UpdatedAt.Should().NotBeNull();
    }
}