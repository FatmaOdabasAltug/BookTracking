namespace BookTracking.Application.Dtos;

public class GroupedAuditLogDto
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public List<AuditLogDto> Logs { get; set; } = new();
}
