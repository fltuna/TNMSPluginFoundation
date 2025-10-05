using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TnmsCentralizedDbPlatform.Shared;

public interface ITnmsRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    IQueryable<T> Query();
}