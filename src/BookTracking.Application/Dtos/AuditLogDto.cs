using BookTracking.Domain.Enums;

namespace BookTracking.Application.Dtos;

public class AuditLogDto : BaseDto
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public AuditType Action { get; set; }
    public string? PropertyName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Description { get; set; }
}