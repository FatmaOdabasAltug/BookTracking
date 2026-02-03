using BookTracking.Domain.Dtos;
using BookTracking.Domain.Entities;
using BookTracking.Domain.Enums;
using BookTracking.Domain.Interfaces;
using BookTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookTracking.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly BookTrackingDbContext _context;

    public AuditLogRepository(BookTrackingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);
    }

    public async Task<Dictionary<string, List<AuditLog>>> GetByFilterGroupedAsync(AuditLogFilterCriteria parameters)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (parameters.EntityType.HasValue)
            query = query.Where(x => x.EntityType == parameters.EntityType.Value);

        if (parameters.EntityId.HasValue)
            query = query.Where(x => x.EntityId == parameters.EntityId.Value);

        if (parameters.Action.HasValue)
            query = query.Where(x => x.Action == parameters.Action.Value);

        if (!string.IsNullOrWhiteSpace(parameters.PropertyName))
        {
            var propertyName = parameters.PropertyName.Trim().ToLower();
            query = query.Where(x => x.PropertyName.ToLower().Contains(propertyName));       
        }
        
        if (!string.IsNullOrWhiteSpace(parameters.OldValue))
        {
            var oldValue = parameters.OldValue.Trim().ToLower();
            query = query.Where(x => x.OldValue != null && x.OldValue.ToLower().Contains(oldValue));
        }
        
        if (!string.IsNullOrWhiteSpace(parameters.NewValue))
        {
            var newValue = parameters.NewValue.Trim().ToLower();
            query = query.Where(x => x.NewValue != null && x.NewValue.ToLower().Contains(newValue));
        }
        
        if (parameters.StartDate.HasValue)
            query = query.Where(x => x.CreatedAt >= parameters.StartDate.Value);

        if (parameters.EndDate.HasValue)
            query = query.Where(x => x.CreatedAt <= parameters.EndDate.Value);

        // Default to Descending (Newest first) unless ASC is explicitly requested
        query = parameters.OrderBy == SortOrder.ASC 
            ? query.OrderBy(x => x.CreatedAt) 
            : query.OrderByDescending(x => x.CreatedAt);

        var paginatedLogs = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        var grouped = parameters.GroupBy switch
        {
            GroupByOption.EntityId => paginatedLogs
                .GroupBy(x => x.EntityId.ToString())
                .ToDictionary(g => g.Key, g => g.ToList()),
                
            GroupByOption.EntityType => paginatedLogs
                .GroupBy(x => x.EntityType.ToString())
                .ToDictionary(g => g.Key, g => g.ToList()),
                
            GroupByOption.Action => paginatedLogs
                .GroupBy(x => x.Action.ToString())
                .ToDictionary(g => g.Key, g => g.ToList()),
                
            GroupByOption.Date => paginatedLogs
                .GroupBy(x => x.CreatedAt.Date.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key, g => g.ToList()),
                
            _ => new Dictionary<string, List<AuditLog>> 
                { { "All", paginatedLogs } }
        };

        return grouped;
    }
}