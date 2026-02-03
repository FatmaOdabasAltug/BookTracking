using BookTracking.Domain.Enums;
using BookTracking.Domain.Constants;

namespace BookTracking.API.Models;

public class FilterAuditLogRequest
{
    public Guid? EntityId { get; set; }
    public EntityType? EntityType { get; set; }
    public AuditType? Action { get; set; }
    public string? PropertyName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public int PageNumber { get; set; } = PageConstants.DefaultPageNumber;
    public int PageSize { get; set; } = PageConstants.DefaultPageSize;
    public SortOrder OrderBy { get; set; } = SortOrder.DESC;
}
