using ArianTel.Core.Entities.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.DAL.EF.Context.DatabaseContext;

namespace ArianTel.DAL.EF.Context.Repositories.Ipg;
public sealed class TerminalInitIpgRequestRepository : RepositoryBase<TerminalInitIpgRequest>, ITerminalInitIpgRepository
{
    public TerminalInitIpgRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}

public sealed class TerminalInitIpgResponseRepository : RepositoryBase<TerminalInitIpgResponse>, ITerminalInitIpgResponseRepository
{
    public TerminalInitIpgResponseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
