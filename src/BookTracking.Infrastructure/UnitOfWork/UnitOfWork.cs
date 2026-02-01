using BookTracking.Domain.Interfaces;
using BookTracking.Infrastructure.Data;

namespace BookTracking.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly BookTrackingDbContext _context;

    public UnitOfWork(BookTrackingDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}