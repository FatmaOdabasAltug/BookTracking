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
        return await _context.Books.AnyAsync(x => x.Isbn == isbn && x.IsActive);
    }
}