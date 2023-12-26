using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ArianTel.DAL.EF.Context.DatabaseContext;
public static class ConfigurationHelper
{
    public static IConfiguration GetConfiguration(string basePath = null!)
    {
        basePath ??= Directory.GetCurrentDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
            .AddEnvironmentVariables();

        return configuration.Build();
    }
}
