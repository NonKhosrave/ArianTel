using System;
using System.Collections.Generic;
using ArianTel.Core.Config;
using ArianTel.Core.Swagger;
using ArianTel.DAL.EF.DependencyInjection;
using ArianTel.Service.DependencyInjection;
using BuildingBlocks.DependencyInjection;
using BuildingBlocks.SeriLog;
using BuildingBlocks.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArianTel.API;
public sealed class Startup
{
    private const string ArabicCorsConfig = "ArianTel";

    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        Configuration = configuration;
        HostEnvironment = hostEnvironment;
    }

    public IConfiguration Configuration { get; }
    public IHostEnvironment HostEnvironment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMinimalMvc();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
        services.RegisterServices();

        services.AddCustomApiVersioning();
        services.AddHttpContextAccessor();

        services.Configure<AppConfig>(Configuration.GetSection(nameof(AppConfig)));
        services.Configure<AppConfig>(Configuration.Bind);
        services.AddConfiguredMsSqlDbContext(Configuration.GetConnectionString("SqlServer"), HostEnvironment);
        //services.AddDbContext(Configuration, HostEnvironment);
        services.AddService(Configuration);
        //services.AddJwtAuthentication(Configuration);

        services.AddHttpClient("sms", client =>
        {
            var baseUrl = Configuration.GetValue<string>("Sms:BaseUrl");
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("traceparent", "");
        }).ConfigurePrimaryHttpMessageHandler(Extensions.ByPassSslCertificate)
            .AddHttpMessageHandler<RequestIdHandler>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(10d));

        services.AddHttpClient("SamanProvider", client =>
        {
            var baseUrl = Configuration.GetValue<string>("SamanProviderSettings:BaseUrl");
            var timeOut = Configuration.GetValue<int?>("SamanProviderSettings:TimeOut");
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(timeOut ?? 30);
        }).ConfigurePrimaryHttpMessageHandler(Extensions.ByPassSslCertificate)
            .AddHttpMessageHandler<RequestIdHandler>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(10d));

        var origin = Configuration.GetSection("Origins").Get<List<string>>();
        services.AddCors(setupAction =>
        {
            setupAction.AddPolicy(ArabicCorsConfig, builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
                builder.WithOrigins(origin?.ToArray()).SetIsOriginAllowedToAllowWildcardSubdomains();
            });
        });
        services.AddCustomDistributeCache(Configuration);
        services.AddMemoryCache();
        services.AddSwagger();
        services.AddSerilogDataCollector();
        services.RegisterMapping();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseStaticFiles();
        app.UseCors(ArabicCorsConfig);
        app.UseSerilogDataCollector();
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs"));

        app.UseRouting();
        app.UseExceptionHandlerMiddleware();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
