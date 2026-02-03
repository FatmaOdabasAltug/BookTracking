namespace BookTracking.API.Models;

public class GroupedAuditLogResponse
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public List<AuditLogResponse> Logs { get; set; } = new();
}
