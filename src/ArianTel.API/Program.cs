using System;
using BuildingBlocks.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ArianTel.API;
public static class Program
{
    public static void Main(string[] args)
    {
        var configName = Environment.GetEnvironmentVariable("ConfigName");
        if (string.IsNullOrEmpty(configName?.Trim()))
            configName = "config name";


        Console.WriteLine($"-- ArianTel.API Service API --- {DateTime.Now}");
        Console.WriteLine($"-- ArianTel.API Config Name {configName}");

        Host.CreateDefaultBuilder(args)
            //.UseCustomSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .ConfigureLogging(Extensions.ConfigureLogging)
                    .ConfigureAppConfiguration((hostingContext, config) =>
                        Extensions.ConfigureAppConfiguration(hostingContext, config, configName))
                    .ConfigureKestrel(Extensions.ConfigureKestrel)
                    .UseStartup<Startup>();
            })
            .Build()
            .Run();

    }
}
