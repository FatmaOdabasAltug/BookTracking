using BookTracking.Domain.Enums;

namespace BookTracking.Domain.Dtos;
public class AuditLogFilterParameters
{
    // Filter parameters
    public Guid? EntityId { get; set; }
    public EntityType? EntityType { get; set; }
    public AuditType? Action { get; set; }
    public string? PropertyName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    // Date range filters
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Pagination and sorting
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = "DESC";
}