using System;
using BuildingBlocks.Behavior;
using BuildingBlocks.Common;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.DependencyInjection;
public static class ServiceCollectionConfigurationHandler
{
    public static IServiceCollection RegisterMapping(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        TypeAdapterConfig.GlobalSettings.Scan(assemblies);

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EventsBehaviour<,>));
        services.AddSingleton<ICustomPublisher, CustomPublisher>();


        var assemblys = AppDomain.CurrentDomain.GetAssemblies();

        services.Scan(selector => selector
            .FromAssemblies(assemblys)
            .AddClasses(filter => filter.AssignableTo(typeof(IValidator<>)).Where(r =>
                !r.IsAbstract && r.IsClass && !r.IsGenericType && r.IsPublic
            ))
            .AsImplementedInterfaces());


        services.Scan(selector => selector
            .FromAssemblies(assemblys)
            .AddClasses(filter => filter.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(selector => selector
            .FromAssemblies(assemblys)
            .AddClasses(filter => filter.AssignableTo<IScopeLifetime>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(selector => selector
            .FromAssemblies(assemblys)
            .AddClasses(filter => filter.AssignableTo<ISingletonLifetime>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        services.Scan(selector => selector
            .FromAssemblies(assemblys)
            .AddClasses(filter => filter.AssignableTo<ITransientLifetime>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(selector => selector
            .FromAssemblies(assemblys)
            .AddClasses(filter => filter.AssignableTo(typeof(EventMessageBaseHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static IServiceCollection RegisterPipeLine<TRequest, TResult, TPipeline>(this IServiceCollection services)
        where TRequest : BaseEntity, IRequest<TResult>
        where TResult : ServiceResContextBase, new()
        where TPipeline : BehaviorBase<TRequest, TResult>
    {
        services.AddTransient<IPipelineBehavior<TRequest, TResult>, TPipeline>();
        return services;
    }
}
