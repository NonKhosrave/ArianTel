using BuildingBlocks.DependencyInjection;
using ArianTel.Core.Abstractions;
using ArianTel.Core.Entities.Ipg;

namespace ArianTel.Core.Repositories.Ipg;
public interface ITerminalVerifyIpgRequestRepository : IRepository<TerminalVerifyIpgRequest>, IScopeLifetime
{
}

public interface ITerminalVerifyIpgResponseRepository : IRepository<TerminalVerifyIpgResponse>, IScopeLifetime
{
}
