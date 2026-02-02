namespace BookTracking.API.Models;

public class BookResponse
{
    public required Guid Id { get; set; }
    public required string Isbn { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime PublishDate { get; set; }
    public bool IsActive { get; set; }
    public required List<Guid> Authors { get; set; }
}