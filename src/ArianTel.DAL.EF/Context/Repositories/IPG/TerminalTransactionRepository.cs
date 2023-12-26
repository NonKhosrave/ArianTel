using ArianTel.Core.Entities.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.DAL.EF.Context.DatabaseContext;

namespace ArianTel.DAL.EF.Context.Repositories.Ipg;
public sealed class TerminalTransactionRepository : RepositoryBase<TerminalTransaction>, ITerminalTransactionRepository
{
    public TerminalTransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
