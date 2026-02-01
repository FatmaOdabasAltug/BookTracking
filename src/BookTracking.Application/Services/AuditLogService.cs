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

    public async Task<IEnumerable<AuditLogDto?>> FilterAuditLogsAsync(FilterAuditLogRequestDto parameters)
    {
        var filterparameters = _mapper.Map<AuditLogFilterParameters>(parameters);
        var auditLogs = await _auditLogRepository.FilterAsync(filterparameters);
        return _mapper.Map<IEnumerable<AuditLogDto?>>(auditLogs);
    }
}