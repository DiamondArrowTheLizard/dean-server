using System.Linq.Expressions;
using Interfaces.Models.Tables;

namespace Interfaces.Handlers.Tables;

public interface ITableHandler
{
    Task<T?> GetByIdAsync<T>(int id) where T : class, ITable;
    Task<IEnumerable<T>> GetAllAsync<T>() where T : class, ITable;
    Task<T> CreateAsync<T>(T entity) where T : class, ITable;
    Task<T> UpdateAsync<T>(T entity) where T : class, ITable;
    Task<bool> DeleteAsync<T>(int id) where T : class, ITable;
    Task<IEnumerable<T>> QueryAsync<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : class, ITable;
    Task<bool> ExistsAsync<T>(int id) where T : class, ITable;
}