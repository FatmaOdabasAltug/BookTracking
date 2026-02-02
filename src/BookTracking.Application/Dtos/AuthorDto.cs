namespace BookTracking.Application.Dtos;

public class AuthorDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public List<Guid> Books { get; set; } = new();
    public List<BookSummaryDto> BookDetails { get; set; } = new();
}