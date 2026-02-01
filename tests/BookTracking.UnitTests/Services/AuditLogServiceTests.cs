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
    public async Task FilterAuditLogsAsync_ShouldReturnLogs()
    {
        var request = new FilterAuditLogRequestDto { PageNumber = 1, PageSize = 10 };
        var filterParams = new AuditLogFilterParameters { PageNumber = 1, PageSize = 10 };
        var logs = new List<AuditLog> { new AuditLog { Description = "Log 1" } };
        var logDtos = new List<AuditLogDto> { new AuditLogDto { Description = "Log 1" } };

        _mockMapper.Setup(m => m.Map<AuditLogFilterParameters>(request)).Returns(filterParams);
        _mockAuditLogRepository.Setup(r => r.FilterAsync(filterParams)).ReturnsAsync(logs);
        _mockMapper.Setup(m => m.Map<IEnumerable<AuditLogDto?>>(logs)).Returns(logDtos);

        var result = await _auditLogService.FilterAuditLogsAsync(request);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First()!.Description.Should().Be("Log 1");
    }
}
