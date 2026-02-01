using System;
using BookTracking.Domain.Common;
using BookTracking.Domain.Enums;

namespace BookTracking.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public AuditType Action { get; set; }
    public string? PropertyName { get; set; } // Changed property name example : "Title", "Author", etc.
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public required string Description { get; set; }
}