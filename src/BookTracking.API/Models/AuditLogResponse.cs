using BookTracking.Domain.Enums;

namespace BookTracking.API.Models;

public class AuditLogResponse
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public AuditType Action { get; set; }
    public string? PropertyName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
