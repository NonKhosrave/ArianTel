using BuildingBlocks.DependencyInjection;
using ArianTel.Core.Abstractions;
using ArianTel.Core.Entities.Wallet;

namespace ArianTel.Core.Repositories.Wallet;
public interface ITransferRepository : IRepository<Transfer>, IScopeLifetime
{
}
