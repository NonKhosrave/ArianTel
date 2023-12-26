using System.Threading;
using System.Threading.Tasks;
using ArianTel.Core.Entities.Wallet;
using ArianTel.Core.Repositories.Wallet;
using ArianTel.DAL.EF.Context.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ArianTel.DAL.EF.Context.Repositories.Wallet;
public sealed class AccountRepository : RepositoryBase<Account>, IAccountRepository
{
    public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Account> GetAccountById(int? userId, CancellationToken cancellationToken)
    {
        var account = await Entities.SingleOrDefaultAsync(a => a.UserId == userId, cancellationToken);
        return account;
    }
}
