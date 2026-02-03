using BookTracking.Domain.Constants;
using BookTracking.Domain.Enums;

namespace BookTracking.Application.Dtos;

public class FilterAuditLogRequestDto
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
    public int PageNumber { get; set; } = PageConstants.DefaultPageNumber;
    public int PageSize { get; set; } = PageConstants.DefaultPageSize;
    public SortOrder OrderBy { get; set; } = SortOrder.DESC;
}