using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.DependencyInjection;
using ArianTel.Core.Abstractions;
using ArianTel.Core.Entities.Wallet;

namespace ArianTel.Core.Repositories.Wallet;
public interface ITransactionRepository : IRepository<Transaction>, IScopeLifetime
{
    Task<List<Transaction>> GetTransactionReport(CancellationToken cancellationToken);
}
