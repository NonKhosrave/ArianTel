using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Services;

namespace ArianTel.Core.Abstractions;
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action) where T : ServiceResContextBase, new();
}
