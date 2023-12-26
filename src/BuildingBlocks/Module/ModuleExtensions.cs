using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Module;
public static class ModuleExtensions
{
    public static void AddModulesServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var modules = assemblies.SelectMany(x => x.GetTypes()).Where(t =>
            t.IsClass && !t.IsAbstract && !t.IsGenericType && !t.IsInterface
            && t.GetConstructor(Type.EmptyTypes) != null
            && typeof(IModuleDefinition).IsAssignableFrom(t))
            .ToList();

        foreach (var module in modules)
        {
            var instantiatedType = (IModuleDefinition)Activator.CreateInstance(module)!;
            instantiatedType.AddModuleServices(services, configuration, webHostEnvironment);
        }

    }

    public static void AddModulesMap(
        this IApplicationBuilder app,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var modules = assemblies.SelectMany(x => x.GetTypes()).Where(t =>
            t.IsClass && !t.IsAbstract && !t.IsGenericType && !t.IsInterface
            && t.GetConstructor(Type.EmptyTypes) != null
            && typeof(IModuleDefinition).IsAssignableFrom(t))
            .ToList();

        foreach (var module in modules)
        {
            var instantiatedType = (IModuleDefinition)Activator.CreateInstance(module)!;
            instantiatedType.MapEndpoints(app, configuration, webHostEnvironment);
        }

    }

}
