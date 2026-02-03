namespace BookTracking.API.Models;

public class CreateBookRequest
{
    public required string Isbn { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateOnly PublishDate { get; set; }
    public required List<Guid> Authors { get; set; }
}