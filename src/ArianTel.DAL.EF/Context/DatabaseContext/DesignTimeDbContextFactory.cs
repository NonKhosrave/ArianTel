using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArianTel.DAL.EF.Context.DatabaseContext;
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var services = new ServiceCollection();
        services.AddOptions();
        services.AddLogging(cfg => cfg.AddConsole().AddDebug());
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton<AuditableEntitiesInterceptor>();

        var configuration = ConfigurationHelper.GetConfiguration(AppContext.BaseDirectory)
            ?.GetConnectionString("SqlServer");
        services.AddSingleton(provider => configuration);

        var serviceProvider = services.BuildServiceProvider();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseConfiguredMsSql(configuration, serviceProvider, null);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
