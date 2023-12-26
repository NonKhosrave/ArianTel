using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ArianTel.Core.Abstractions;

public interface IRepository<TEntity>
    where TEntity : class, IDbBaseEntity
{
    IUnitOfWork UnitOfWork { get; }
    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    ValueTask<TEntity> FindByIdAsync(CancellationToken cancellationToken, params object[] ids);

    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, object>> orderBy = null, bool isDescending = false,
        CancellationToken cancellationToken = default);

    Task<List<TResult>> FindWithPagination<TResult>(Expression<Func<TEntity, bool>> predicate, int pageIndex,
        int pageSize, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, object>> orderBy = null,
        bool isDescending = false, CancellationToken cancellationToken = default)
        where TResult : class, new();
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entityList, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entityList);
    Task<int> BulkRemove(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    void Detach(TEntity entity);
    void Attach(TEntity entity);
    T GetShadowPropertyValue<T>(TEntity entity, string propertyName) where T : IConvertible;
    object GetShadowPropertyValue(TEntity entity, string propertyName);
}
