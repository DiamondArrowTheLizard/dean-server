using Interfaces.Handlers.Tables;
using Interfaces.Models.Tables;
using Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Data;

namespace Models.Handlers.Tables;

public class TableHandler : ITableHandler
{
    private readonly IServiceProvider _serviceProvider;

    public TableHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<T?> GetByIdAsync<T>(int id) where T : class, ITable
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITableRepository<T>>();
        return await repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, ITable
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITableRepository<T>>();
        return await repository.GetAllAsync();
    }

    public async Task<T> CreateAsync<T>(T entity) where T : class, ITable
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITableRepository<T>>();
        return await repository.AddAsync(entity);
    }

    public async Task<T> UpdateAsync<T>(T entity) where T : class, ITable
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITableRepository<T>>();
        await repository.UpdateAsync(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync<T>(int id) where T : class, ITable
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITableRepository<T>>();
        await repository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(Func<IQueryable<T>, IQueryable<T>> query) where T : class, ITable
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DeanDbContext>();
        var dbSet = context.Set<T>();
        var result = query(dbSet);
        return await result.ToListAsync();
    }

    public async Task<bool> ExistsAsync<T>(int id) where T : class, ITable
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITableRepository<T>>();
        return await repository.ExistsAsync(id);
    }
}