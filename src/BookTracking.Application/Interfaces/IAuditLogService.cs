using BookTracking.Application.Dtos;
namespace BookTracking.Application.Interfaces;
public interface IAuditLogService
{
    Task CreateLogAsync(AuditLogDto auditLog);
    Task<IEnumerable<AuditLogDto?>> GetFilteredAuditLogsAsync(AuditLogFilterCriteriaDto parameters);
}