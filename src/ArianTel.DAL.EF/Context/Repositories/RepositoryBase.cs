using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ArianTel.Core.Abstractions;
using ArianTel.DAL.EF.Context.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ArianTel.DAL.EF.Context.Repositories;
public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
    where TEntity : class, IDbBaseEntity
{
    protected readonly ApplicationDbContext DbBaseContext;
    protected DbSet<TEntity> Entities { get; }

    protected RepositoryBase(ApplicationDbContext dbContext)
    {
        DbBaseContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        Entities = DbBaseContext.Set<TEntity>();
    }

    public IUnitOfWork UnitOfWork => DbBaseContext;
    protected IQueryable<TEntity> Table => Entities;
    protected IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await Entities.Where(predicate).AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual ValueTask<TEntity> FindByIdAsync(CancellationToken cancellationToken, params object[] ids)
    {
        return Entities.FindAsync(ids, cancellationToken);
    }

    public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderBy = null, bool isDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = TableNoTracking.Where(predicate);
        if (orderBy != null)
        {
            query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        }
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TResult>> FindWithPagination<TResult>(Expression<Func<TEntity, bool>> predicate, int pageIndex,
        int pageSize, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, object>> orderBy = null, bool isDescending = false, CancellationToken cancellationToken = default)
        where TResult : class, new()
    {
        var query = TableNoTracking.Where(predicate);

        if (orderBy != null)
        {
            query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        }
        return await query
            .Select(selector)
            .Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Entities.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entityList, CancellationToken cancellationToken = default)
    {
        await Entities.AddRangeAsync(entityList, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        Entities.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entityList)
    {
        Entities.UpdateRange(entityList);
    }

    public virtual async Task<int> BulkRemove(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Entities.Where(predicate).ExecuteDeleteAsync(cancellationToken);
    }

    public virtual void Detach(TEntity entity)
    {
        var entry = DbBaseContext.Entry(entity);
        entry.State = EntityState.Detached;
    }

    public virtual void Attach(TEntity entity)
    {
        if (DbBaseContext.Entry(entity).State == EntityState.Detached)
            Entities.Attach(entity);
    }
    public T GetShadowPropertyValue<T>(TEntity entity, string propertyName) where T : IConvertible
    {
        var value = DbBaseContext.Entry(entity).Property(propertyName).CurrentValue;
        return value != null
                   ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
                   : default;
    }

    public object GetShadowPropertyValue(TEntity entity, string propertyName) =>
        DbBaseContext.Entry(entity).Property(propertyName).CurrentValue;
}
