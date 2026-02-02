namespace BookTracking.API.Models;

public class UpdateAuthorRequest
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
}
