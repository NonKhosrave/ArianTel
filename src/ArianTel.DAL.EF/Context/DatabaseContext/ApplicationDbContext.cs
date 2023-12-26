using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ArianTel.Core.Abstractions;
using ArianTel.Core.Abstractions.AuditableEntity;
using ArianTel.Core.Utilities;
using ArianTel.DAL.EF.Configurations;

namespace ArianTel.DAL.EF.Context.DatabaseContext;
public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private IDbContextTransaction _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction != null;
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // it should be placed here, otherwise it will rewrite the following settings!
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddCustomIdentityMappings();
        modelBuilder.SetDecimalPrecision();
        modelBuilder.AddDateTimeUtcKindConverter();
        modelBuilder.RegisterAllEntities<IDbBaseEntity>(typeof(IDbBaseEntity).Assembly);
        // This should be placed here, at the end.
        modelBuilder.AddAuditableShadowProperties();
    }

    public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();

    public async Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action) where T : ServiceResContextBase, new()
    {
        var isCreateTran = await BeginTransactionAsync();

        if (!isCreateTran)
            return await action();

        try
        {
            var res = await action();

            await _currentTransaction.CommitAsync();

            return res;
        }
        catch
        {
            await _currentTransaction.RollbackAsync();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null) return false;

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        return true;
    }
}
