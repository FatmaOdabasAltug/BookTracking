using BookTracking.Domain.Entities;
using BookTracking.Domain.Interfaces;
using BookTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookTracking.Infrastructure.Repositories;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    private readonly BookTrackingDbContext _context;
    public AuthorRepository(BookTrackingDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Author?>> GetAuthorListAsync(IEnumerable<Guid> ids)
    {
        return await _context.Authors
            .Where(a => ids.Contains(a.Id))
            .ToListAsync();
    }

    public async Task<Author?> GetByIdWithBooksAsync(Guid id)
    {
        return await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public override async Task<IEnumerable<Author>> GetAllAsync()
    {
        return await _context.Authors
            .Include(a => a.Books)
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToListAsync();
    }
}