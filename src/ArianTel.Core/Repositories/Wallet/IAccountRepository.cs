using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.DependencyInjection;
using ArianTel.Core.Abstractions;
using ArianTel.Core.Entities.Wallet;

namespace ArianTel.Core.Repositories.Wallet;
public interface IAccountRepository : IRepository<Account>, IScopeLifetime
{
    Task<Account> GetAccountById(int? userId, CancellationToken cancellationToken);
}
