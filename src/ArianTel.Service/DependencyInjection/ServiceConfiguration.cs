using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ArianTel.Core.Config;

namespace ArianTel.Service.DependencyInjection;
public static class ServiceConfiguration
{
    public const string EmailConfirmationTokenProviderName = "ConfirmEmail";

    public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IPrincipal>(provider =>
                                           provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User ??
                                           ClaimsPrincipal.Current);


        return services;
    }

    public static AppConfig GetSiteSettings(this IConfiguration configuration)
    {
        return configuration.Get<AppConfig>();
    }
}
