using ArianTel.Core.Entities.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.DAL.EF.Context.DatabaseContext;

namespace ArianTel.DAL.EF.Context.Repositories.Ipg;
public sealed class TerminalReverseIpgRequestRepository : RepositoryBase<TerminalReverseIpgRequest>, ITerminalReverseIpgRequestRepository
{
    public TerminalReverseIpgRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}

public sealed class TerminalReverseIpgResponseRepository : RepositoryBase<TerminalReverseIpgResponse>, ITerminalReverseIpgResponseRepository
{
    public TerminalReverseIpgResponseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
