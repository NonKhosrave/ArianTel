using BuildingBlocks.DependencyInjection;
using ArianTel.Core.Abstractions;
using ArianTel.Core.Entities.Ipg;

namespace ArianTel.Core.Repositories.Ipg;
public interface ITerminalInitIpgRepository : IRepository<TerminalInitIpgRequest>, IScopeLifetime
{
}

public interface ITerminalInitIpgResponseRepository : IRepository<TerminalInitIpgResponse>, IScopeLifetime
{
}
