using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ArianTel.Core.Abstractions.AuditableEntity;
using ArianTel.Core.Utilities;

namespace ArianTel.DAL.EF.Context.DatabaseContext;
public sealed class AuditableEntitiesInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditableEntitiesInterceptor> _logger;

    public AuditableEntitiesInterceptor(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditableEntitiesInterceptor> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData == null)
        {
            throw new ArgumentNullException(nameof(eventData));
        }

        BeforeSaveTriggers(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData == null)
        {
            throw new ArgumentNullException(nameof(eventData));
        }

        BeforeSaveTriggers(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void BeforeSaveTriggers(DbContext context)
    {
        ValidateEntities(context);
        ApplyAudits(context?.ChangeTracker);
    }

    private void ValidateEntities(DbContext context)
    {
        var errors = context?.GetValidationErrors();
        if (string.IsNullOrWhiteSpace(errors))
        {
            return;
        }

        _logger.CompileLogMessage(LogLevel.Error, errors);
        throw new InvalidOperationException(errors);
    }

    private void ApplyAudits(ChangeTracker changeTracker)
    {
        if (changeTracker is null)
        {
            return;
        }

        var props = _httpContextAccessor?.GetShadowProperties();
        changeTracker.SetAuditableEntityPropertyValues(props);
    }
}
