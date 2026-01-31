using BookTracking.Domain.Common;
using BookTracking.Domain.Interfaces;
using BookTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookTracking.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly BookTrackingDbContext _context;

    public GenericRepository(BookTrackingDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(Guid id) 
        => await _context.Set<T>().FirstOrDefaultAsync(b => b.Id  == id);

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            entity.IsActive = false;
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}