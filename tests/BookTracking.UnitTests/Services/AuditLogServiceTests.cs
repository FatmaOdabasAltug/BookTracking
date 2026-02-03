using AutoMapper;
using BookTracking.Application.Dtos;
using BookTracking.Application.Services;
using BookTracking.Application.Interfaces;
using BookTracking.Domain.Dtos;
using BookTracking.Domain.Entities;
using BookTracking.Domain.Enums;
using BookTracking.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookTracking.UnitTests.Services;

public class AuditLogServiceTests
{
    private readonly Mock<IAuditLogRepository> _mockAuditLogRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AuditLogService _auditLogService;

    public AuditLogServiceTests()
    {
        _mockAuditLogRepository = new Mock<IAuditLogRepository>();
        _mockMapper = new Mock<IMapper>();

        _auditLogService = new AuditLogService(
            _mockAuditLogRepository.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task CreateLogAsync_ShouldAddLog()
    {
        var logDto = new AuditLogDto 
        { 
            Action = AuditType.Create, 
            EntityType = EntityType.Book, 
            EntityId = Guid.NewGuid(),
            Description = "Test Log"
        };

        await _auditLogService.CreateLogAsync(logDto);

        _mockAuditLogRepository.Verify(r => r.AddAsync(It.Is<AuditLog>(l => 
            l.Action == logDto.Action && 
            l.EntityType == logDto.EntityType && 
            l.Description == logDto.Description
        )), Times.Once);
    }

    [Fact]
    public async Task FilterAuditLogsAsync_ShouldReturnGroupedLogs()
    {
        var request = new AuditLogFilterCriteriaDto { PageNumber = 1, PageSize = 10, GroupBy = GroupByOption.None };
        var filterParams = new AuditLogFilterCriteria { PageNumber = 1, PageSize = 10, GroupBy = GroupByOption.None };
        var logs = new Dictionary<string, List<AuditLog>> 
        { 
            { "All", new List<AuditLog> { new AuditLog { Description = "Log 1", EntityId = Guid.NewGuid(), EntityType = EntityType.Book, Action = AuditType.Create } } }
        };
        var logDtos = new List<AuditLogDto> { new AuditLogDto { Description = "Log 1" } };
        var groupedDtos = new List<GroupedAuditLogDto> 
        { 
            new GroupedAuditLogDto { GroupKey = "All", GroupName = "All Logs", TotalCount = 1, Logs = logDtos }
        };

        _mockMapper.Setup(m => m.Map<AuditLogFilterCriteria>(request)).Returns(filterParams);
        _mockAuditLogRepository.Setup(r => r.GetByFilterGroupedAsync(filterParams)).ReturnsAsync(logs);
        _mockMapper.Setup(m => m.Map<List<AuditLogDto>>(It.IsAny<List<AuditLog>>())).Returns(logDtos);

        var result = await _auditLogService.GetFilteredAuditLogsGroupedAsync(request);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        var firstGroup = result.First();
        firstGroup.GroupKey.Should().Be("All");
        firstGroup.TotalCount.Should().Be(1);
    }
}
