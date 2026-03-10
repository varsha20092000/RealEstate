using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Interfaces;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public async Task UpdateAsync(T entity)
        => _dbSet.Update(entity);

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
            _dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
        => await _dbSet.FindAsync(id) != null;
}