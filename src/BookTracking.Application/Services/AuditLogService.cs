using AutoMapper;
using BookTracking.Application.Dtos;
using BookTracking.Application.Interfaces;
using BookTracking.Domain.Dtos;
using BookTracking.Domain.Entities;
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

    public async Task<IEnumerable<AuditLogDto?>> GetFilteredAuditLogsAsync(AuditLogFilterCriteriaDto parameters)
    {
        var filterParameters = _mapper.Map<AuditLogFilterCriteria>(parameters);
        var auditLogs = await _auditLogRepository.GetByFilterAsync(filterParameters);
        return _mapper.Map<IEnumerable<AuditLogDto?>>(auditLogs);
    }
}