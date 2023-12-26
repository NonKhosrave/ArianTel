using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArianTel.DAL.EF.Context.DatabaseContext;
public static class MsSqlServiceCollectionExtensions
{
    public static void UseConfiguredMsSql(
        this DbContextOptionsBuilder optionsBuilder, string connectionString, IServiceProvider serviceProvider, IHostEnvironment hostEnvironment)
    {
        if (optionsBuilder == null)
        {
            throw new ArgumentNullException(nameof(optionsBuilder));
        }

        if (connectionString == null)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        optionsBuilder.UseSqlServer(
            connectionString,
            sqlServerOptionsBuilder =>
            {
                sqlServerOptionsBuilder.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                //sqlServerOptionsBuilder.EnableRetryOnFailure(10,
                //    TimeSpan.FromSeconds(7),
                //    null);
                sqlServerOptionsBuilder.MigrationsAssembly(typeof(MsSqlServiceCollectionExtensions).Assembly.FullName);
            });
        optionsBuilder.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntitiesInterceptor>());
        optionsBuilder.ConfigureWarnings(warnings =>
        {
            warnings.Log(
                (CoreEventId.LazyLoadOnDisposedContextWarning, LogLevel.Warning),
                (CoreEventId.DetachedLazyLoadingWarning, LogLevel.Warning),
                (CoreEventId.ManyServiceProvidersCreatedWarning, LogLevel.Warning),
                (CoreEventId.SensitiveDataLoggingEnabledWarning, LogLevel.Information)
            );
        });
        optionsBuilder.EnableSensitiveDataLogging().EnableDetailedErrors();
        if (hostEnvironment != null && hostEnvironment.IsDevelopment())
            optionsBuilder.LogTo(Console.WriteLine, minimumLevel: LogLevel.Information);
    }
}
