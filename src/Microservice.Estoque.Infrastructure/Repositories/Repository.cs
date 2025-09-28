using Microsoft.EntityFrameworkCore;
using Microservice.Estoque.Application.Interfaces;
using Microservice.Estoque.Infrastructure.Data.Context;

namespace Microservice.Estoque.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly EstoqueDbContext _ctx;
    protected readonly DbSet<T> _set;
    public Repository(EstoqueDbContext ctx)
    {
        _ctx = ctx;
        _set = ctx.Set<T>();
    }
    public async Task AddAsync(T entity)
    {
        await _set.AddAsync(entity);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        await _ctx.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await _set.AsNoTracking().ToListAsync();

    public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);

    public async Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        await _ctx.SaveChangesAsync();
    }
}
