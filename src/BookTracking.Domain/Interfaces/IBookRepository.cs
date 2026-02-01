using BookTracking.Domain.Entities;

namespace BookTracking.Domain.Interfaces;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<bool> AnyByIsbnAsync(string isbn);
    Task<Book?> GetByIdWithAuthorsAsync(Guid id);

}