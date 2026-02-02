namespace BookTracking.API.Models;

public class BookSummaryResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
}
