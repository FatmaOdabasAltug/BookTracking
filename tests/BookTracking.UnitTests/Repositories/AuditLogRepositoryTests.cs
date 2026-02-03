using BookTracking.Domain.Dtos;
using BookTracking.Domain.Entities;
using BookTracking.Domain.Enums;
using BookTracking.Infrastructure.Data;
using BookTracking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

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
    public async Task AddAsync_ShouldAddLog()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuditLogRepository(context);
        
        var log = new AuditLog 
        { 
            Id = Guid.NewGuid(), 
            EntityType = EntityType.Book, 
            Action = AuditType.Create, 
            EntityId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Description = "Created book"
        };

        await repo.AddAsync(log);
        await context.SaveChangesAsync();

        var count = await context.AuditLogs.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task FilterAsync_ShouldFilterByParameters()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuditLogRepository(context);

        var log1 = new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Book, Action = AuditType.Create, PropertyName = "Title", CreatedAt = DateTime.UtcNow.AddDays(-1), EntityId=Guid.NewGuid(), Description="Log 1" };
        var log2 = new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Author, Action = AuditType.Update, PropertyName = "Name", CreatedAt = DateTime.UtcNow, EntityId=Guid.NewGuid(), Description="Log 2" };
        var log3 = new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Book, Action = AuditType.Delete, PropertyName = "Description", CreatedAt = DateTime.UtcNow.AddDays(1), EntityId=Guid.NewGuid(), Description="Log 3" };

        await context.AuditLogs.AddRangeAsync(log1, log2, log3);
        await context.SaveChangesAsync();


        var result1 = await repo.FilterAsync(new AuditLogFilterParameters { EntityType = EntityType.Book, PageNumber=1, PageSize=10, OrderBy=SortOrder.ASC });
        result1.Should().HaveCount(2);


        var result2 = await repo.FilterAsync(new AuditLogFilterParameters { Action = AuditType.Update, PageNumber=1, PageSize=10, OrderBy=SortOrder.ASC });
        result2.Should().HaveCount(1);
        result2.First()!.Id.Should().Be(log2.Id);

        var result3 = await repo.FilterAsync(new AuditLogFilterParameters { PropertyName = "Title", PageNumber=1, PageSize=10, OrderBy=SortOrder.ASC });
        result3.Should().HaveCount(1);

        var result4 = await repo.FilterAsync(new AuditLogFilterParameters { StartDate = DateTime.UtcNow.AddHours(-1), EndDate = DateTime.UtcNow.AddHours(1), PageNumber=1, PageSize=10, OrderBy=SortOrder.ASC });
        result4.Should().HaveCount(1);
    }

    [Fact]
    public async Task FilterAsync_ShouldHandlePagination()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuditLogRepository(context);

        for(int i=0; i<5; i++)
        {
             await context.AuditLogs.AddAsync(new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Book, Action = AuditType.Create, EntityId=Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddMinutes(i), Description=$"Log {i}" });
        }
        await context.SaveChangesAsync();

        var result = await repo.FilterAsync(new AuditLogFilterParameters { PageNumber = 2, PageSize = 2, OrderBy=SortOrder.ASC });
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task FilterAsync_ShouldFilterByCombinedParameters()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuditLogRepository(context);

        // Match: Type=Book AND Action=Create
        var log1 = new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Book, Action = AuditType.Create, Description="Log 1", EntityId=Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        // No Match: Type=Book but Action=Update
        var log2 = new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Book, Action = AuditType.Update, Description="Log 2", EntityId=Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
         // No Match: Type=Author
        var log3 = new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Author, Action = AuditType.Create, Description="Log 3", EntityId=Guid.NewGuid(), CreatedAt = DateTime.UtcNow };

        await context.AuditLogs.AddRangeAsync(log1, log2, log3);
        await context.SaveChangesAsync();

        var result = await repo.FilterAsync(new AuditLogFilterParameters { EntityType = EntityType.Book, Action = AuditType.Create, PageNumber=1, PageSize=10, OrderBy=SortOrder.ASC });
        
        result.Should().HaveCount(1);
        result.First()!.Id.Should().Be(log1.Id);
    }
    [Fact]
    public async Task FilterAsync_ShouldSortByNewestByDefault()
    {
        var options = GetOptions();
        using var context = new BookTrackingDbContext(options);
        var repo = new AuditLogRepository(context);

        var log1 = new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Book, Action = AuditType.Create, Description="Log 1", EntityId=Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddMinutes(-10) };
        var log2 = new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Book, Action = AuditType.Create, Description="Log 2", EntityId=Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        var log3 = new AuditLog { Id = Guid.NewGuid(), EntityType = EntityType.Book, Action = AuditType.Create, Description="Log 3", EntityId=Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddMinutes(-5) };

        await context.AuditLogs.AddRangeAsync(log1, log2, log3);
        await context.SaveChangesAsync();

        // Use default parameters (OrderBy should be "DESC")
        var result = await repo.FilterAsync(new AuditLogFilterParameters { PageNumber=1, PageSize=10 });
        
        result.Should().HaveCount(3);
        var list = result.ToList();
        list[0]!.Id.Should().Be(log2.Id); // Newest
        list[2]!.Id.Should().Be(log1.Id);  // Oldest
        list[1]!.Id.Should().Be(log3.Id);      // Middle
    }
}