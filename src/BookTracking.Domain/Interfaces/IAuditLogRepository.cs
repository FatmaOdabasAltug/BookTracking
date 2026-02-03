using BookTracking.Domain.Dtos;
using BookTracking.Domain.Entities;
namespace BookTracking.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog);
    Task<IEnumerable<AuditLog?>> GetByFilterAsync(AuditLogFilterCriteria parameters);

}