using BookTracking.Domain.Entities;
namespace BookTracking.Domain.Interfaces;

public interface IAuthorRepository
{
    Task<Author> GetByIdAsync(int id);
    Task<IEnumerable<Author>> GetAllAsync();
    Task AddAsync(Author author);
    void Update(Author author);
    void Delete(Author author);
}