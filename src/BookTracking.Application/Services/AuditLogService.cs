using AutoMapper;
using BookTracking.Application.Dtos;
using BookTracking.Application.Interfaces;
using BookTracking.Domain.Dtos;
using BookTracking.Domain.Entities;
using BookTracking.Domain.Enums;
using BookTracking.Domain.Interfaces;

namespace BookTracking.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public AuditLogService(IAuditLogRepository auditLogRepository, IMapper mapper)
    {
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task CreateLogAsync(AuditLogDto auditLog)
    {
        await _auditLogRepository.AddAsync(new AuditLog
            {
                Action = auditLog.Action,
                EntityType = auditLog.EntityType,
                EntityId = auditLog.EntityId,
                Description = auditLog.Description,
                PropertyName = auditLog.PropertyName,
                OldValue = auditLog.OldValue,
                NewValue = auditLog.NewValue
            });
    }

    public async Task<IEnumerable<GroupedAuditLogDto>> GetFilteredAuditLogsGroupedAsync(AuditLogFilterCriteriaDto parameters)
    {
        var filterParameters = _mapper.Map<AuditLogFilterCriteria>(parameters);
        var groupedLogs = await _auditLogRepository.GetByFilterGroupedAsync(filterParameters);

        var result = new List<GroupedAuditLogDto>();

        foreach (var group in groupedLogs)
        {
            var groupedDto = new GroupedAuditLogDto
            {
                GroupKey = group.Key,
                GroupName = GenerateGroupName(group.Key, group.Value.FirstOrDefault(), parameters.GroupBy),
                TotalCount = group.Value.Count,
                Logs = _mapper.Map<List<AuditLogDto>>(group.Value)
            };
            result.Add(groupedDto);
        }

        return result;
    }

    private string GenerateGroupName(string groupKey, AuditLog? firstLog, GroupByOption groupBy)
    {
        if (firstLog == null) return groupKey;

        return groupBy switch
        {
            GroupByOption.EntityId => $"{firstLog.EntityType}: {groupKey}",
            GroupByOption.EntityType => $"All {groupKey}s",
            GroupByOption.Date => DateTime.Parse(groupKey).ToString("MMMM d, yyyy"),
            GroupByOption.Action => $"{groupKey} Operations",
            _ => "All Logs"
        };
    }
}