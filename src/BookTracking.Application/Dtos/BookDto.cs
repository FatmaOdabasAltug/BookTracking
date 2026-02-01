namespace BookTracking.Application.Dtos;

public class BookDto : BaseDto
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public List<Guid> Authors { get; set; } = new();
}