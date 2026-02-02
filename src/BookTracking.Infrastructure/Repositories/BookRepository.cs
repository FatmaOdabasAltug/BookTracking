using BookTracking.Domain.Entities;
using BookTracking.Domain.Interfaces;
using BookTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookTracking.Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    private readonly BookTrackingDbContext _context;

    public BookRepository(BookTrackingDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> AnyByIsbnAsync(string isbn)
    {
        return await _context.Books.AnyAsync(x => x.Isbn == isbn);
    }

    public async Task<Book?> GetByIdWithAuthorsAsync(Guid id)
    {
        return await _context.Books.
            Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public override async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
            .Include(b => b.Authors)
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToListAsync();
    }
}