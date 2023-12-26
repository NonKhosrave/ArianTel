using ArianTel.Core.Entities.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.DAL.EF.Context.DatabaseContext;

namespace ArianTel.DAL.EF.Context.Repositories.Ipg;
public sealed class TerminalCallBackRepository : RepositoryBase<TerminalCallBack>, ITerminalCallBackRepository
{
    public TerminalCallBackRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
