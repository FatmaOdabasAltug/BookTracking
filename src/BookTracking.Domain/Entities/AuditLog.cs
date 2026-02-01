using System;
using BookTracking.Domain.Common;
using BookTracking.Domain.Enums;

namespace BookTracking.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public AuditType Action { get; set; }
    public string PropertyName { get; set; } = string.Empty; // Changed property name example : "Title", "Author","Description"
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Description { get; set; } = string.Empty;
}