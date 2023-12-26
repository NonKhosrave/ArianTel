using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using BuildingBlocks.Behavior;
using BuildingBlocks.DateTimeHelper;
using BuildingBlocks.Filter;
using BuildingBlocks.Model;
using BuildingBlocks.SeriLog;
using BuildingBlocks.Service.Cache;
using BuildingBlocks.Service.Interface;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using StackExchange.Redis;

namespace BuildingBlocks.Service;
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Add middleware for exception handling.
    ///     if add config ExceptionShowFullError:true to appsettings.json, show full error in 500(dev and proc)
    /// </summary>
    /// <param name="app"></param>
    /// <param name="useStringEnumConverter"> use string enum convertor</param>
    public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app,
        bool useStringEnumConverter = true)
    {
        app.UseMiddleware<CustomExceptionMiddleware>(useStringEnumConverter);
    }

    public static void AddExceptionHandlerMiddleware(this IServiceCollection app)
    {
        app.AddHttpContextAccessor();

        AddChannelService(app);

        AddWorkers(app);
    }

    public static void AddChannelService(this IServiceCollection app)
    {
        app.AddSingleton(typeof(ChannelService<>));
    }

    public static void AddWorkers(this IServiceCollection app)
    {
        app.AddSingleton<IWorkers, Workers>();
    }

    public static void AddMessageBroker(this IServiceCollection app)
    {
        app.AddSingleton<IMessageBrokerDataService, MessageBrokerDataService>();
    }

    /// <summary>
    ///     Add minimal mvc(mvc without razor,..)
    /// </summary>
    /// <param name="useApiResult">add api result filter</param>
    /// <param name="useStringEnumConverter">add string enum converter to json setting</param>
    public static IMvcBuilder AddMinimalMvc(this IServiceCollection services, bool useApiResult = true,
        bool useStringEnumConverter = true, bool useYeKeFilter = true)
    {
        Regex.CacheSize += 100;
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        return services.AddControllers(options =>
        {
            if (useApiResult)
                options.Filters.Add<ApiResultFilterAttribute>();
            if (useYeKeFilter)
            {
                options.Filters.Add<ApplyCorrectYeKeFilterAttribute>();
            }
        })
            .AddJsonOptions(op =>
            {
                op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                op.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                op.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                op.JsonSerializerOptions.WriteIndented = false;
                op.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                op.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                op.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });
    }

    public static void AddCustomDistributeCache(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheConfig = configuration.GetSection("CacheConfig").Get<CacheConfig>();

        if (!cacheConfig.IsEnabled)
        {
            services.AddSingleton<ICustomDistributeCache, FakeDistributeCache>();
            return;
        }

        var redisCache = cacheConfig.RedisCache;
        var inMemory = cacheConfig.InMemory;

        if (redisCache.IsEnabled)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    ConnectTimeout = 2500,
                    ClientName = redisCache.InstanceName,
                    SyncTimeout = 2500,
                    AsyncTimeout = 2500,
                    ConnectRetry = 1,
                    Password = redisCache.Password,
                    Ssl = false
                };

                if (!string.IsNullOrEmpty(redisCache.ServiceName))
                    options.ConfigurationOptions.ServiceName = redisCache.ServiceName;

                foreach (var con in redisCache.Connection) options.ConfigurationOptions.EndPoints.Add(con);
            });
        }
        else if (inMemory.IsEnabled)
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddSingleton<ICustomDistributeCache, FakeDistributeCache>();
            return;
        }

        services.AddSingleton<ICustomDistributeCache, CustomDistributeCache>();
    }


    public static void AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(option =>
        {
            option.DefaultApiVersion = new ApiVersion(1, 0);
            option.ReportApiVersions = false;
            option.AssumeDefaultVersionWhenUnspecified = true;
            option.ErrorResponses = new ApiVersionErrorResponse();
        });
    }

    public static void AddSerilogDataCollector(this IServiceCollection app)
    {
        app.AddTransient<RequestIdHandler>();
        app.AddSingleton<EnvPackages>();
    }

    public static void UseSerilogDataCollector(this IApplicationBuilder app)
    {
        app.UseMiddleware<LogHeaderMiddleware>();
    }

    public static IHostBuilder UseCustomSerilog(this IHostBuilder builder)
    {
        return builder.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithExceptionDetails());
    }
}

