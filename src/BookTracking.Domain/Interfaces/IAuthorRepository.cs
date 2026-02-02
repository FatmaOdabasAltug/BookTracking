using System.Runtime.CompilerServices;
using BookTracking.Domain.Entities;

namespace BookTracking.Domain.Interfaces;

public interface IAuthorRepository : IGenericRepository<Author>
{
    Task<IEnumerable<Author?>> GetAuthorListAsync(IEnumerable<Guid> ids);
    Task<Author?> GetByIdWithBooksAsync(Guid id);
    Task<bool> IsAuthorNameExistsAsync(string name);

}