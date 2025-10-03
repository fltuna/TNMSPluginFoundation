using Microsoft.EntityFrameworkCore;
using TnmsCentralizedDbPlatform.Shared;

namespace TnmsCentralizedDbPlatform;

internal class TnmsRepository<T>(DbContext context) : ITnmsRepository<T>
    where T : class
{
    protected readonly DbContext Context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }
}