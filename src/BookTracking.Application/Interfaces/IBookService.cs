using BookTracking.Application.Dtos;
namespace BookTracking.Application.Interfaces;

public interface IBookService
{
    Task<BookDto> CreateBookAsync(BookDto dto);
    Task<BookDto> UpdateBookAsync(BookDto dto);
    Task DeleteBookAsync(Guid id);
    Task<IEnumerable<BookDto>> GetAllBooksAsync();
}