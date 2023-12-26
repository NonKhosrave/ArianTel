using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ArianTel.Core.Abstractions;
using ArianTel.DAL.EF.Context.DatabaseContext;

namespace ArianTel.DAL.EF.DependencyInjection;
public static class ServiceConfiguration
{
    public static IServiceCollection AddConfiguredMsSqlDbContext(this IServiceCollection services, string connectionString, IHostEnvironment hostEnvironment)
    {
        services.AddScoped<IUnitOfWork>(serviceProvider =>
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            SetCascadeOnSaveChanges(context);
            return context;
        });
        services.AddSingleton<AuditableEntitiesInterceptor>();
        services.AddDbContextPool<ApplicationDbContext>(
            (serviceProvider, optionsBuilder) => optionsBuilder.UseConfiguredMsSql(connectionString, serviceProvider, hostEnvironment));
        return services;
    }

    private static void SetCascadeOnSaveChanges<TDbContext>(TDbContext context)
        where TDbContext : DbContext
    {
        //https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete
        context.ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
        context.ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
    }
}
