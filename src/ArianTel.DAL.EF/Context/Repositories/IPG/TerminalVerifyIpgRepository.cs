using ArianTel.Core.Entities.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.DAL.EF.Context.DatabaseContext;

namespace ArianTel.DAL.EF.Context.Repositories.Ipg;
public sealed class TerminalVerifyIpgRequestRepository : RepositoryBase<TerminalVerifyIpgRequest>, ITerminalVerifyIpgRequestRepository
{
    public TerminalVerifyIpgRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}

public sealed class TerminalVerifyIpgResponseRepository : RepositoryBase<TerminalVerifyIpgResponse>, ITerminalVerifyIpgResponseRepository
{
    public TerminalVerifyIpgResponseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
