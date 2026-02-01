using BookTracking.Domain.Entities;
using BookTracking.Infrastructure.Data;
using BookTracking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;
using BookTracking.Domain.Enums;

namespace BookTracking.UnitTests.Repositories;

public class AuditLogRepositoryTests
{
    private DbContextOptions<BookTrackingDbContext> GetOptions()
    {
        return new DbContextOptionsBuilder<BookTrackingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;
    }


    [Fact]
    public async Task CreateAsync_ShouldAddAuditLog()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuditLogRepository(context);
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = EntityType.Book,
            Action = AuditType.Update,
            PropertyName = "Title",
            OldValue = "Old Book Title",
            NewValue = "New Book Title",
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(auditLog);

        var result = await context.AuditLogs.FindAsync(auditLog.Id);
        result.Should().NotBeNull();
        result!.PropertyName.Should().Be("Title");
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnFilteredAuditLogs()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuditLogRepository(context);
        var auditLog1 = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = EntityType.Book,
            Action = AuditType.Create,
            PropertyName = "Author",
            OldValue = null,
            NewValue = "Author A",
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        var auditLog2 = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = EntityType.Book,
            Action = AuditType.Update,
            PropertyName = "Title",
            OldValue = "Old Title",
            NewValue = "New Title",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        await context.AuditLogs.AddRangeAsync(auditLog1, auditLog2);
        await context.SaveChangesAsync();
        var parameters = new BookTracking.Domain.Dtos.AuditLogFilterParameters
        {
            EntityType = EntityType.Book,
            Action = AuditType.Update,
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "ASC"
        };
        var results = await repo.SearchAsync(parameters);
        results.Should().HaveCount(1);
        results.First().PropertyName.Should().Be("Title");
    }
    
}