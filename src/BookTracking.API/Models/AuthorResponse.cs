namespace BookTracking.API.Models;

public class AuthorResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public List<Guid>? Books { get; set; }
}
