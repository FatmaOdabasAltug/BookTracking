using BookTracking.Application.Dtos;

public interface IAuthorService
{
    Task<AuthorDto> CreateAuthorAsync(AuthorDto dto);
    Task<AuthorDto> UpdateAuthorAsync(AuthorDto dto);
    Task DeleteAuthorAsync(Guid id);
}